using ArcaeaUnlimitedAPI.Lib;
using Flandre.Core.Utils;
using SkiaSharp;
using YukiChan.Plugins.Arcaea.Models;
using YukiChan.Utils;

namespace YukiChan.Plugins.Arcaea.Images;

public static partial class ArcaeaImageGenerator
{
    public static async Task<byte[]> Single(ArcaeaUser user, ArcaeaRecord record, AuaClient client,
        bool nya, Logger? logger = null)
    {
        return await Task.Run(() =>
        {
            var imageInfo = new SKImageInfo(900, 1520);
            using var surface = SKSurface.Create(imageInfo);
            var canvas = surface.Canvas;

            var cover = client
                .GetSongCover(record.SongId, record.JacketOverride, record.Difficulty, nya, logger)
                .Result;

            var (colorLight, colorDark, colorBorderLight, colorBorderDark, colorInnerLight, colorInnerDark)
                = DifficultyColors[(int)record.Difficulty];

            {
                var bgPath = "Assets/Arcaea/Images/SingleBackground.jpg";
                using var background = SKBitmap.Decode(bgPath);

                if (background is null)
                    logger?.Warning($"资源文件缺失: {bgPath}");

                using var scaledBackground = new SKBitmap(900, 1520);
                background?.ScalePixels(scaledBackground, SKFilterQuality.Medium);

                canvas.DrawBitmap(scaledBackground, 0, 0);
            }

            {
                // 背景
                using var cardPaint = new SKPaint
                {
                    Color = new SKColor(255, 255, 255, 200),
                    IsAntialias = true,
                    ImageFilter = SKImageFilter.CreateDropShadow(
                        0, 0, 35, 35, new SKColor(0, 0, 0, 50))
                };
                canvas.DrawRoundRect(100, 100, 700, 1305, 30, 30, cardPaint);
            }

            {
                // 处理曲绘
                using var originalSongCover = SKBitmap.Decode(cover);
                using var scaledSongCover = new SKBitmap(500, 500);
                originalSongCover.ScalePixels(scaledSongCover, SKFilterQuality.Medium);

                canvas.DrawBitmap(scaledSongCover, 200, 200);
            }

            {
                using var titlePaint = new SKPaint
                {
                    Color = SKColor.Parse("#333333"),
                    Typeface = FontBold,
                    IsAntialias = true,
                    TextSize = 53
                };
                canvas.DrawCenteredText(record.Name, 160, 770, titlePaint, 580);
            }

            {
                var clearImgPath = "Assets/Arcaea/Images/" + record.ClearType switch
                {
                    ArcaeaClearType.NormalClear => "ClearTC.png",
                    ArcaeaClearType.EasyClear => "ClearTC.png",
                    ArcaeaClearType.HardClear => "ClearTC.png",

                    ArcaeaClearType.TrackLost => "ClearTL.png",
                    ArcaeaClearType.FullRecall => "ClearFR.png",
                    ArcaeaClearType.PureMemory => "ClearPM.png",

                    _ => "ClearTC.png"
                };
                using var originalClearImg = SKBitmap.Decode(clearImgPath);

                if (originalClearImg.Height > 77)
                {
                    using var scaledClearImg = new SKBitmap(600, 74);
                    originalClearImg.ScalePixels(scaledClearImg, SKFilterQuality.Medium);
                    canvas.DrawBitmap(scaledClearImg, 150, 790);
                }
                else
                {
                    using var scaledClearImg = new SKBitmap(600, 53);
                    originalClearImg.ScalePixels(scaledClearImg, SKFilterQuality.Medium);
                    canvas.DrawBitmap(scaledClearImg, 150, 800);
                }
            }

            {
                using var textPaint = new SKPaint
                {
                    Color = SKColor.Parse("#333333"),
                    TextSize = 85,
                    IsAntialias = true,
                    Typeface = GeoSans
                };
                canvas.DrawCenteredText(record.Score.FormatScore(), 200, 944, textPaint, 500);
            }


            if (record.Grade == ArcaeaGrade.EX)
            {
                var exImg = SKBitmap.Decode("Assets/Arcaea/Images/GradeEX.png");
                var scaledExImg = new SKBitmap(132, 111);
                exImg.ScalePixels(scaledExImg, SKFilterQuality.Medium);
                canvas.DrawBitmap(scaledExImg, 384, 980);
            }
            else
            {
                var gradeImg = SKBitmap.Decode($"Assets/Arcaea/Images/Grade{record.Grade}.png");
                canvas.DrawBitmap(gradeImg, 335, 980);
            }

            {
                // Pure/Far/Lost 信息
                using var textPaint = new SKPaint
                {
                    TextSize = 40,
                    IsAntialias = true,
                    Typeface = GeoSans
                };
                // Pure
                textPaint.Color = SKColor.Parse("#6f3a5f");
                canvas.DrawText($"Pure / {record.PureCount} (+{record.ShinyPureCount})", 300, 1150, textPaint);

                // Far
                textPaint.Color = SKColor.Parse("#c19c00");
                canvas.DrawText($"Far / {record.FarCount}", 300, 1200, textPaint);

                // Lost
                textPaint.Color = SKColor.Parse("#bb2b43");
                canvas.DrawText($"Lost / {record.LostCount}", 300, 1250, textPaint);
            }

            {
                // 距离当前天数
                using var rectPaint = new SKPaint
                {
                    Color = SKColor.Parse("#dddddd"),
                    IsAntialias = true
                };
                using var textPaint = new SKPaint
                {
                    Color = SKColor.Parse("#333333"),
                    TextSize = 38,
                    IsAntialias = true,
                    Typeface = FontRegular
                };

                var milis = (long)(DateTime.Now - DateTime.UnixEpoch).TotalMilliseconds;
                var days = new TimeSpan(0, 0, 0, 0, (int)(milis - record.TimePlayed)).TotalDays;

                logger?.Debug($"milis: {milis}    days: {days}");

                canvas.DrawRoundRect(150, 1295, 600, 60, 10, 10, rectPaint);
                canvas.DrawCenteredText(
                    $"{(int)days}d",
                    655, 1337, textPaint, 80);
            }

            {
                // 难度条
                using var rectPaint = new SKPaint
                {
                    Color = SKColor.Parse(colorDark),
                    IsAntialias = true
                };
                using var textPaint = new SKPaint
                {
                    Color = SKColor.Parse("#ffffff"),
                    TextSize = 38,
                    IsAntialias = true,
                    Typeface = FontRegular
                };

                canvas.DrawRoundRect(150, 1295, 490, 60, 10, 10, rectPaint);
                canvas.DrawLimitedText(
                    $"{record.Difficulty} {record.RatingText} [{record.Rating}]",
                    332, 1337, textPaint, 334);
            }

            {
                // 获得 ptt
                using var rectPaint = new SKPaint
                {
                    Color = SKColor.Parse(colorLight)
                };
                using var textPaint = new SKPaint
                {
                    Color = SKColors.White,
                    TextSize = 38,
                    IsAntialias = true,
                    Typeface = FontBold
                };
                canvas.DrawRoundRect(150, 1295, 162, 60, 10, 10, rectPaint);
                canvas.DrawText($"{record.Potential:0.0000}", 166, 1337, textPaint);
            }

            {
                using var textPaint = new SKPaint
                {
                    Color = SKColors.White,
                    TextSize = 38,
                    IsAntialias = true,
                    Typeface = FontRegular
                };
                canvas.DrawCenteredText("Generated by YukiChan", 100, 1477, textPaint, 700);
            }

            var data = surface
                .Snapshot()
                .Encode(SKEncodedImageFormat.Jpeg, 70)
                .ToArray();

            return data;
        });
    }
}