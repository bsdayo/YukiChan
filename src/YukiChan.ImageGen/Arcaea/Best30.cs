﻿using ArcaeaUnlimitedAPI.Lib;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using YukiChan.ImageGen.Utils;
using YukiChan.Shared;
using YukiChan.Shared.Arcaea;
using YukiChan.Shared.Arcaea.Models;
using YukiChan.Shared.Database.Models.Arcaea;
using YukiChan.Shared.Utils;

namespace YukiChan.ImageGen.Arcaea;

public partial class ArcaeaImageGenerator
{
    public byte[] Best30(ArcaeaBest30 best30,
        ArcaeaUserPreferences pref, AuaClient? client = null,
        ILogger? logger = null)
    {
        var imageInfo = new SKImageInfo(3400, 6200);
        using var surface = SKSurface.Create(imageInfo);
        var canvas = surface.Canvas;

        {
            var bgPath = $"{YukiDir.ArcaeaAssets}/images/best30-background-{(pref.Dark ? "dark" : "light")}.jpg";
            using var background = SKBitmap.Decode(bgPath);

            if (background is null)
                logger?.LogWarning($"资源文件缺失: {bgPath}");

            using var scaledBackground = new SKBitmap(
                3400, (background?.Height ?? 6200) * (3400 / (background?.Width ?? 3400)));
            background?.ScalePixels(scaledBackground, SKFilterQuality.Medium);

            canvas.DrawBitmap(scaledBackground, 0, 0);
        }

        {
            // 名称 / ptt
            using var paint = new SKPaint
            {
                Color = pref.Dark ? SKColors.White : SKColors.Black,
                TextSize = 128,
                IsAntialias = true,
                Typeface = TitilliumWeb_SemiBold
            };
            canvas.DrawText($"{best30.User.Name} ({best30.User.Potential})",
                295, 255, paint);
        }

        {
            // 账号信息
            using var paint = new SKPaint
            {
                Color = pref.Dark ? SKColors.White : SKColors.Black,
                TextSize = 62,
                IsAntialias = true,
                Typeface = TitilliumWeb_SemiBold
            };

            double best10Total = 0;

            for (var i = 0; i < 10; i++)
            {
                if (i > best30.Records.Length - 1)
                    break;

                best10Total += best30.Records[i].Potential;
            }

            canvas.DrawText(
                $"B30Avg / {best30.Best30Avg:0.0000}   " +
                $"R10Avg / {best30.Recent10Avg:0.0000}   " +
                $"MaxPtt / {(best10Total + 30 * best30.Best30Avg) / 40:0.0000}",
                295, 365, paint);
        }

        {
            // Player Best30
            using var paint = new SKPaint
            {
                Color = pref.Dark ? SKColors.White : SKColors.Black,
                TextSize = 130,
                IsAntialias = true,
                Typeface = TitilliumWeb_SemiBold
            };
            canvas.DrawText("Player Best30", 2300, 290, paint);
        }

        {
            // 分割线
            using var linePaint = new SKPaint
            {
                Color = pref.Dark
                    ? new SKColor(255, 255, 255, 128)
                    : new SKColor(0, 0, 0, 128)
            };
            canvas.DrawRect(120, 500, 3160, 10, linePaint);
            canvas.DrawRect(120, 4698, 3160, 10, linePaint);
        }

        {
            // 时间
            using var paint = new SKPaint
            {
                Color = pref.Dark ? SKColors.White : SKColors.Black,
                IsAntialias = true,
                TextSize = 80,
                Typeface = TitilliumWeb_Regular
            };
            var time = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");

            canvas.DrawText($"Generated by YukiChan @ {time}",
                897, 6100, paint);
        }

        for (var col = 0; col < 3; col++)
        for (var row = 0; row < 10; row++)
        {
            var index = col * 10 + row;

            if (index > best30.Records.Length - 1)
                break;

            var record = best30.Records[index];

            var songCover = ArcaeaUtils
                .GetSongCover(client, record.SongId, record.JacketOverride, record.Difficulty, pref.Nya, logger)
                .GetAwaiter().GetResult();

            DrawMiniScoreCard(canvas,
                100 + col * 1100, 635 + row * 400, record, songCover, index + 1, pref.Dark);
        }

        // Overflow
        if (best30.HasOverflow)
            for (var col = 0; col < 3; col++)
            for (var row = 0; row < 3; row++)
            {
                var index = col * 3 + row;

                if (index > best30.OverflowRecords!.Length - 1)
                    break;

                var record = best30.OverflowRecords![index];

                var songCover = ArcaeaUtils
                    .GetSongCover(client, record.SongId, record.JacketOverride, record.Difficulty, pref.Nya, logger)
                    .GetAwaiter().GetResult();

                DrawMiniScoreCard(canvas,
                    100 + col * 1100, 4840 + row * 400, record, songCover, index + 31, pref.Dark);
            }

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 70);
        return data.ToArray();
    }

    public void DrawMiniScoreCard(SKCanvas canvas, int x, int y,
        ArcaeaRecord record, byte[] songCover, int rank = 0, bool dark = false)
    {
        var (colorLight, colorDark, colorBorderLight, colorBorderDark, colorInnerLight, colorInnerDark)
            = DifficultyColors[(int)record.Difficulty];

        {
            // 背景
            using var backgroundPaint = new SKPaint
            {
                Color = dark
                    ? new SKColor(40, 40, 40, 200)
                    : new SKColor(255, 255, 255, 200),
                IsAntialias = true,
                ImageFilter = SKImageFilter.CreateDropShadow(
                    0, 0, 35, 35, new SKColor(0, 0, 0, 50))
            };
            canvas.DrawRoundRect(x, y, 1000, 320, 20, 20, backgroundPaint);
        }

        // 处理曲绘
        {
            using var originalSongCover = SKBitmap.Decode(songCover);
            using var scaledSongCover = new SKBitmap(290, 290);
            originalSongCover.ScalePixels(scaledSongCover, SKFilterQuality.Medium);

            canvas.DrawBitmap(scaledSongCover, x + 15, y + 15);
        }

        // 排名
        if (rank != 0)
        {
            using var textPaint = new SKPaint
            {
                TextSize = 45,
                IsAntialias = true,
                Typeface = TitilliumWeb_SemiBold
            };
            using var rectPaint = new SKPaint();
            textPaint.Color = SKColor.Parse(rank switch
            {
                < 3 => "#333333",
                3 => "#ffffff",
                _ => dark ? "#ffffff" : "#333333"
            });
            rectPaint.Color = SKColor.Parse(rank switch
            {
                1 => "#ffcc00",
                2 => "#c0c0c0",
                3 => "#a57c50",
                _ => dark ? "#333333" : "#dddddd"
            });

            canvas.DrawRoundRect(x + 320, y + 15, 665, 60, 10, 10, rectPaint);
            canvas.DrawText($"#{rank}", x + 895, y + 61, textPaint);
        }

        {
            // 难度条
            using var rectPaint = new SKPaint
            {
                Color = SKColor.Parse(dark ? colorInnerDark : colorDark),
                IsAntialias = true
            };

            using var textPaint = new SKPaint
            {
                Color = SKColor.Parse("#ffffff"),
                TextSize = 45,
                IsAntialias = true,
                Typeface = TitilliumWeb_Regular
            };

            canvas.DrawRoundRect(x + 320, y + 15, rank != 0 ? 560 : 665, 60, 10, 10, rectPaint);
            canvas.DrawLimitedText(
                $"{record.Difficulty} {record.RatingText} [{record.Rating}]",
                x + 526, y + 61, textPaint, rank != 0 ? 339 : 444);

            if (dark)
            {
                using var borderPaint = new SKPaint
                {
                    Color = SKColor.Parse(colorBorderDark),
                    IsAntialias = true,
                    IsStroke = true,
                    StrokeWidth = 3
                };
                canvas.DrawRoundRect(x + 320, y + 15, rank != 0 ? 560 : 665, 60, 10, 10, borderPaint);
            }
        }

        {
            // 获得 ptt
            using var rectPaint = new SKPaint
            {
                Color = SKColor.Parse(dark ? colorInnerLight : colorLight),
                IsAntialias = true
            };

            using var textPaint = new SKPaint
            {
                Color = SKColors.White,
                TextSize = 45,
                IsAntialias = true,
                Typeface = TitilliumWeb_SemiBold
            };
            canvas.DrawRoundRect(x + 320, y + 15, 191, 60, 10, 10, rectPaint);
            canvas.DrawText($"{record.Potential:0.0000}", x + 335, y + 61, textPaint);

            if (dark)
            {
                using var borderPaint = new SKPaint
                {
                    Color = SKColor.Parse(colorBorderLight),
                    IsAntialias = true,
                    IsStroke = true,
                    StrokeWidth = 3
                };
                canvas.DrawRoundRect(x + 320, y + 15, 191, 60, 10, 10, borderPaint);
            }
        }

        {
            // 曲名
            using var textPaint = new SKPaint
            {
                Color = dark ? SKColors.White : SKColor.Parse("#333333"),
                TextSize = 60,
                IsAntialias = true,
                Typeface = TitilliumWeb_SemiBold
            };

            canvas.DrawLimitedText(ArcaeaUtils.ReplaceNotSupportedChar(record.Name),
                x + 335, y + 136, textPaint, 635);
        }

        {
            // 得分
            // 若理论值则绘制蓝色阴影
            if (record.ShinyPureCount == record.PureCount &&
                record is { FarCount: 0, LostCount: 0 })
            {
                using var maxPaint = new SKPaint
                {
                    Color = SKColor.Parse("#7fdfff"),
                    TextSize = 97,
                    IsAntialias = true,
                    Typeface = TitilliumWeb_Regular
                };
                canvas.DrawText(record.Score.FormatScore(), x + 340, y + 241, maxPaint);
            }

            using var textPaint = new SKPaint
            {
                Color = dark ? SKColors.White : SKColor.Parse("#333333"),
                TextSize = 97,
                IsAntialias = true,
                Typeface = TitilliumWeb_Regular
            };
            canvas.DrawText(record.Score.FormatScore(), x + 335, y + 236, textPaint);
        }

        {
            // Pure/Far/Lost 信息
            using var textPaint = new SKPaint
            {
                Color = SKColors.White,
                TextSize = 40,
                IsAntialias = true,
                Typeface = TitilliumWeb_Regular
            };

            // 彩色字体
            // Pure
            if (!dark) textPaint.Color = SKColor.Parse("#6f3a5f");
            canvas.DrawLimitedText($"Pure / {record.PureCount} (+{record.ShinyPureCount})",
                x + 335, y + 296, textPaint, 260);

            // Far
            if (!dark) textPaint.Color = SKColor.Parse("#c19c00");
            canvas.DrawLimitedText($"Far / {record.FarCount}",
                x + 616, y + 296, textPaint, 98);

            // Lost
            if (!dark) textPaint.Color = SKColor.Parse("#bb2b43");
            canvas.DrawLimitedText($"Lost / {record.LostCount}",
                x + 765, y + 296, textPaint, 110);

            // Past Days
            if (!dark) textPaint.Color = SKColors.Gray;
            textPaint.TextAlign = SKTextAlign.Right;
            canvas.DrawLimitedText(DateTime.Now.GetPastDays(record.TimePlayed, true),
                x + 980, y + 296, textPaint, record.LostCount < 100 ? 60 : 52);
        }
    }
}