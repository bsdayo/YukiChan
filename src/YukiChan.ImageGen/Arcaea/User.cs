using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using ArcaeaUnlimitedAPI.Lib;
using ArcaeaUnlimitedAPI.Lib.Models;
using ArcaeaUnlimitedAPI.Lib.Responses;
using BrotliSharpLib;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using LiveChartsCore.SkiaSharpView.SKCharts;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using Websocket.Client;
using YukiChan.ImageGen.Utils;
using YukiChan.Shared;
using YukiChan.Shared.Arcaea;
using YukiChan.Shared.Arcaea.Factories;
using YukiChan.Shared.Database.Models.Arcaea;
using YukiChan.Shared.Utils;
using Timer = System.Timers.Timer;

namespace YukiChan.ImageGen.Arcaea;

public partial class ArcaeaImageGenerator
{
    public async Task<byte[]> User(AuaUserInfoContent user, ArcaeaUserPreferences pref, AuaClient auaClient,
        int lastDays, bool smooth, ILogger logger)
    {
        var imageInfo = new SKImageInfo(3400, 2000);
        using var surface = SKSurface.Create(imageInfo);
        var canvas = surface.Canvas;

        {
            // 背景
            var bgPath = $"{YukiDir.ArcaeaAssets}/images/best30-background-{(pref.Dark ? "dark" : "light")}.jpg";
            using var background = SKBitmap.Decode(bgPath);

            if (background is null)
                logger.LogWarning($"资源文件缺失: {bgPath}");

            using var scaledBackground = new SKBitmap(
                3400, (background?.Height ?? 6200) * (3400 / (background?.Width ?? 3400)));
            background?.ScalePixels(scaledBackground, SKFilterQuality.Medium);

            canvas.DrawBitmap(scaledBackground, 0, -1800);
        }

        {
            // 主卡片
            using var cardPaint = new SKPaint
            {
                Color = pref.Dark
                    ? new SKColor(40, 40, 40, 200)
                    : new SKColor(255, 255, 255, 200),
                IsAntialias = true,
                ImageFilter = SKImageFilter.CreateDropShadow(
                    0, 0, 35, 35, new SKColor(0, 0, 0, 50))
            };
            canvas.DrawRoundRect(100, 100, 3200, 1800, 30, 30, cardPaint);
        }

        {
            // 立绘
            var charImage = await auaClient.GetCharImage(
                user.AccountInfo.Character, user.AccountInfo.IsCharUncapped, logger);
            using var charBitmap = SKBitmap.Decode(charImage);
            using var resized = new SKBitmap(1550, 1550);
            charBitmap.ScalePixels(resized, SKFilterQuality.Medium);
            canvas.DrawBitmap(resized, 1920, 155);
        }

        double scopedMax = 0, scopedMin = 0, startPtt = 0, endPtt = 0;

        try
        {
            // 图表
            logger.LogDebug("Getting chart image...");
            (var chartImage, scopedMax, scopedMin, startPtt, endPtt) =
                await GetRatingRecordsChartImage(
                    user.AccountInfo.Code, user.AccountInfo.Rating, 1900, 1320,
                    pref, lastDays, smooth, logger);
            logger.LogDebug("Chart image got successfully.");

            canvas.DrawImage(chartImage, 200, 480);
            logger.LogDebug("Chart image drawn.");
            chartImage.Dispose();
        }
        catch (Exception e)
        {
            logger.LogError(e, "");
        }

        {
            // 名字和 ptt
            using var textPaint = new SKPaint
            {
                Color = pref.Dark ? SKColors.White : SKColor.Parse("#333333"),
                TextSize = 90,
                IsAntialias = true,
                Typeface = TitilliumWeb_SemiBold
            };
            canvas.DrawText(
                $"{user.AccountInfo.Name} ({
                    (user.AccountInfo.Rating >= 0
                        ? (user.AccountInfo.Rating / 100d).ToString("F2")
                        : "?")})",
                200, 290, textPaint);
            textPaint.TextSize = 45;

            var descSb = new StringBuilder()
                .Append($"JoinDate / {user.AccountInfo.JoinDate.FormatTimestamp(true)}")
                .Append($"    Max / {scopedMax:F2}")
                .Append($"    Min / {scopedMin:F2}")
                .Append($"    {startPtt:F2} -> {endPtt:F2}");
            canvas.DrawText(descSb.ToString(), 205, 385, textPaint);
        }

        {
            // 最近游玩
            var cover = await auaClient.GetSongCover(user.RecentScore![0].SongId, user.SongInfo![0].JacketOverride,
                (ArcaeaDifficulty)user.RecentScore[0].Difficulty, pref.Nya, logger);
            DrawMiniScoreCard(canvas, 2200, 1480,
                ArcaeaRecordFactory.FromAua(user.RecentScore[0], user.SongInfo[0]), cover, 0, pref.Dark);
        }

        {
            // copyright
            using var textPaint = new SKPaint
            {
                Color = pref.Dark ? SKColors.White : SKColor.Parse("#333333"),
                TextSize = 60,
                IsAntialias = true,
                Typeface = TitilliumWeb_Regular
            };
            canvas.DrawCenteredText("Generated by YukiChan & Chart data from esterTion",
                0, 1970, textPaint, 3400);
        }

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 70);
        return data.ToArray();
    }

    private Task<(SKImage, double, double, double, double)> GetRatingRecordsChartImage(string userId,
        int userPtt, int width,
        int height,
        ArcaeaUserPreferences pref, int lastDays, bool smooth, ILogger logger)
    {
        var tcs = new TaskCompletionSource<(SKImage, double, double, double, double)>();
        var timer = new Timer(20000);
        var client = new WebsocketClient(new Uri("wss://arc.estertion.win:616"));

        var userPttColorIndex = userPtt switch
        {
            < 350 => 0,
            >= 350 and < 700 => 1,
            >= 700 and < 1100 => 2,
            >= 110 => 3
        };

        void StopEverything()
        {
            timer.Stop();
            client.Stop(WebSocketCloseStatus.Empty, string.Empty).Wait();
            client.Dispose();
            timer.Close();
            timer.Dispose();
            logger.LogDebug("Everything stopped.");
        }

        client.MessageReceived.Subscribe(message =>
        {
            if (message.MessageType == WebSocketMessageType.Text)
            {
                logger.LogDebug($"Received text message: {message.Text}");
                var err = message.Text switch
                {
                    "invalid id" => "用户不存在",
                    "timeout" => "连接超时",
                    _ => null
                };
                if (err is not null)
                {
                    StopEverything();
                    tcs.SetException(new Exception(err));
                }
            }

            if (message.MessageType != WebSocketMessageType.Binary) return;
            var decompressed = Brotli.DecompressBuffer(message.Binary, 0, message.Binary.Length);
            var json = JsonDocument.Parse(Encoding.UTF8.GetString(decompressed));
            if (json.RootElement.GetProperty("cmd").GetString() != "userinfo") return;

            var ratingRecords = json.RootElement
                .GetProperty("data")
                .GetProperty("rating_records")
                .Deserialize<JsonElement[][]>()!;

            var dtpsList = new List<DateTimePoint>();
            foreach (var elements in ratingRecords)
            {
                var date = elements[0].GetString()!;
                var year = int.Parse(date[..2]) + 2000;
                var month = int.Parse(date[2..4]);
                var day = int.Parse(date[4..]);
                var val = elements[1].GetInt32();

                dtpsList.Add(new DateTimePoint(new DateTime(year, month, day), val / 100d));
            }

            var now = DateTime.Now;
            dtpsList = dtpsList
                .Where(dtp => (now - dtp.DateTime).TotalDays <= lastDays)
                .ToList();

            var xAxes = new[]
            {
                new Axis
                {
                    Labeler = value => new DateTime((long)value).ToString("yyyy.MM.dd"),
                    LabelsRotation = 20,
                    UnitWidth = TimeSpan.FromDays(1).Ticks,
                    MinStep = TimeSpan.FromDays(1).Ticks,
                    LabelsPaint = new SolidColorPaint(pref.Dark ? SKColors.White : SKColor.Parse("#333333")),
                    TextSize = 40,
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray)
                    {
                        StrokeThickness = 4,
                        PathEffect = new DashEffect(new[] { 12f, 12f })
                    }
                }
            };

            var scopedMax = dtpsList.Max(d => d.Value)!.Value;
            var scopedMin = dtpsList.Min(d => d.Value)!.Value;
            var max = (int)(scopedMax * 100);
            var min = (int)(scopedMin * 100);

            var yAxesMinStep = (max - min) switch
            {
                <= 16 => 0.01,
                > 16 and <= 26 => 0.02,
                > 26 and <= 46 => 0.03,
                > 46 and <= 96 => 0.05,
                > 96 and <= 196 => 0.10,
                > 196 and <= 296 => 0.15,
                > 296 => 0.20
            };

            logger.LogDebug($"Max: {max}, Min: {min}, MinStep: {yAxesMinStep}");

            var yAxes = new[]
            {
                new Axis
                {
                    Labeler = value => value.ToString("F2"),
                    LabelsPaint = new SolidColorPaint(pref.Dark ? SKColors.White : SKColor.Parse("#333333")),
                    TextSize = 40,
                    MinStep = yAxesMinStep,
                    // MinLimit = min == 0 ? 0 : ((min - 2) / 100d),
                    // MaxLimit = (max + 2) / 100d,
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray)
                    {
                        StrokeThickness = 4,
                        PathEffect = new DashEffect(new[] { 12f, 12f })
                    }
                }
            };

            var series = new LineSeries<DateTimePoint>
            {
                Values = dtpsList,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(SKColor.Parse(DifficultyColors[userPttColorIndex].ColorDark))
                    { StrokeThickness = 10 },
                Fill = new SolidColorPaint(SKColor.Parse(DifficultyColors[userPttColorIndex].ColorLight).WithAlpha(100))
            };

            if (!smooth) series.LineSmoothness = 0;

            var chart = new SKCartesianChart
            {
                Width = width,
                Height = height,
                Series = new[] { series },
                XAxes = xAxes,
                YAxes = yAxes,
                Background = SKColors.Transparent
            };

            logger.LogDebug("Chart initialized.");
            StopEverything();
            tcs.SetResult((chart.GetImage(), scopedMax, scopedMin,
                dtpsList[0].Value!.Value, dtpsList[^1].Value!.Value));
        });

        timer.Elapsed += (_, _) =>
        {
            StopEverything();
            tcs.SetException(new Exception("连接超时。"));
        };


        timer.Start();
        client.Start().Wait();
        logger.LogDebug("Client started.");

        client.Send(userId);
        logger.LogDebug("Message sent.");

        return tcs.Task;
    }
}