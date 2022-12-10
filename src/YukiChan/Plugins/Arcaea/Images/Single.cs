using ArcaeaUnlimitedAPI.Lib;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using YukiChan.Shared;
using YukiChan.Shared.Database.Models.Arcaea;
using YukiChan.Shared.Models.Arcaea;
using YukiChan.Utils;

namespace YukiChan.Plugins.Arcaea.Images;

public static partial class ArcaeaImageGenerator
{
    public static async Task<byte[]> Single(ArcaeaUser user, ArcaeaRecord record, AuaClient client,
        ArcaeaUserPreferences pref, ILogger? logger = null)
    {
        return await Task.Run(() =>
        {
            var imageInfo = new SKImageInfo(900, 1520);
            using var surface = SKSurface.Create(imageInfo);
            var canvas = surface.Canvas;

            var cover = client
                .GetSongCover(record.SongId, record.JacketOverride, record.Difficulty, pref.Nya, logger)
                .Result;

            var (colorLight, colorDark, colorBorderLight, colorBorderDark, colorInnerLight, colorInnerDark)
                = DifficultyColors[(int)record.Difficulty];

            {
                var bgPath = $"{YukiDir.ArcaeaAssets}/images/single-background.jpg";
                using var background = SKBitmap.Decode(bgPath);

                if (background is null)
                    logger?.LogWarning($"资源文件缺失: {bgPath}");

                using var scaledBackground = new SKBitmap(900, 1520);
                background?.ScalePixels(scaledBackground, SKFilterQuality.Medium);

                canvas.DrawBitmap(scaledBackground, 0, 0);
            }

            {
                // 背景
                using var cardPaint = new SKPaint
                {
                    Color = pref.Dark
                        ? new SKColor(40, 40, 40, 200)
                        : new SKColor(255, 255, 255, 200),
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
                    Color = pref.Dark ? SKColors.White : SKColor.Parse("#333333"),
                    Typeface = TitilliumWeb_SemiBold,
                    IsAntialias = true,
                    TextSize = 53
                };
                canvas.DrawCenteredText(ArcaeaUtils.ReplaceNotSupportedChar(record.Name),
                    160, 770, titlePaint, 580);
            }

            {
                var clearImgPath = $"{YukiDir.ArcaeaAssets}/images/" + record.ClearType switch
                {
                    ArcaeaClearType.NormalClear => "clear-tc.png",
                    ArcaeaClearType.EasyClear => "clear-tc.png",
                    ArcaeaClearType.HardClear => "clear-tc.png",

                    ArcaeaClearType.TrackLost => "clear-tl.png",
                    ArcaeaClearType.FullRecall => "clear-fr.png",
                    ArcaeaClearType.PureMemory => "clear-pm.png",

                    _ => "clear-tc.png"
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
                    Color = pref.Dark ? SKColors.White : SKColor.Parse("#333333"),
                    TextSize = 85,
                    IsAntialias = true,
                    Typeface = GeoSans
                };
                canvas.DrawCenteredText(record.Score.FormatScore(), 200, 944, textPaint, 500);
            }


            if (record.Grade == ArcaeaGrade.EX)
            {
                var exImg = SKBitmap.Decode($"{YukiDir.ArcaeaAssets}/images/grade-ex.png");
                var scaledExImg = new SKBitmap(132, 111);
                exImg.ScalePixels(scaledExImg, SKFilterQuality.Medium);
                canvas.DrawBitmap(scaledExImg, 384, 980);
            }
            else
            {
                var gradeImg =
                    SKBitmap.Decode($"{YukiDir.ArcaeaAssets}/images/grade-{record.Grade.ToString().ToLower()}.png");
                canvas.DrawBitmap(gradeImg, 335, 980);
            }

            {
                // Pure/Far/Lost 信息
                using var textPaint = new SKPaint
                {
                    TextSize = 40,
                    IsAntialias = true,
                    Typeface = GeoSans,
                    Color = SKColors.White
                };
                // Pure
                if (!pref.Dark) textPaint.Color = SKColor.Parse("#6f3a5f");
                canvas.DrawText($"Pure / {record.PureCount} (+{record.ShinyPureCount})", 300, 1150, textPaint);

                // Far
                if (!pref.Dark) textPaint.Color = SKColor.Parse("#c19c00");
                canvas.DrawText($"Far / {record.FarCount}", 300, 1200, textPaint);

                // Lost
                if (!pref.Dark) textPaint.Color = SKColor.Parse("#bb2b43");
                canvas.DrawText($"Lost / {record.LostCount}", 300, 1250, textPaint);
            }

            {
                // 距离当前天数
                using var rectPaint = new SKPaint
                {
                    Color = SKColor.Parse(pref.Dark ? "#333333" : "#dddddd"),
                    IsAntialias = true
                };
                using var textPaint = new SKPaint
                {
                    Color = SKColor.Parse(pref.Dark ? "#ffffff" : "#333333"),
                    TextSize = 38,
                    IsAntialias = true,
                    Typeface = TitilliumWeb_Regular
                };

                var milis = (long)(DateTime.Now - DateTime.UnixEpoch).TotalMilliseconds;
                var days = new TimeSpan(0, 0, 0, 0, (int)(milis - record.TimePlayed)).TotalDays;

                canvas.DrawRoundRect(150, 1295, 600, 60, 10, 10, rectPaint);
                canvas.DrawCenteredText(
                    $"{(int)days}d",
                    655, 1337, textPaint, 80);
            }

            {
                // 难度条
                using var rectPaint = new SKPaint
                {
                    Color = SKColor.Parse(pref.Dark ? colorInnerDark : colorDark),
                    IsAntialias = true
                };
                using var textPaint = new SKPaint
                {
                    Color = SKColor.Parse("#ffffff"),
                    TextSize = 38,
                    IsAntialias = true,
                    Typeface = TitilliumWeb_Regular
                };

                canvas.DrawRoundRect(150, 1295, 490, 60, 10, 10, rectPaint);
                canvas.DrawLimitedText(
                    $"{record.Difficulty} {record.RatingText} [{record.Rating}]",
                    332, 1337, textPaint, 334);

                // Border
                if (pref.Dark)
                {
                    using var borderPaint = new SKPaint
                    {
                        Color = SKColor.Parse(colorBorderDark),
                        IsAntialias = true,
                        IsStroke = true,
                        StrokeWidth = 3
                    };
                    canvas.DrawRoundRect(150, 1295, 490, 60, 10, 10, borderPaint);
                }
            }

            {
                // 获得 ptt
                using var rectPaint = new SKPaint
                {
                    Color = SKColor.Parse(pref.Dark ? colorInnerLight : colorLight)
                };
                using var textPaint = new SKPaint
                {
                    Color = SKColors.White,
                    TextSize = 38,
                    IsAntialias = true,
                    Typeface = TitilliumWeb_SemiBold
                };
                canvas.DrawRoundRect(150, 1295, 162, 60, 10, 10, rectPaint);
                canvas.DrawText($"{record.Potential:0.0000}", 166, 1337, textPaint);

                // Border
                if (pref.Dark)
                {
                    using var borderPaint = new SKPaint
                    {
                        Color = SKColor.Parse(colorBorderLight),
                        IsAntialias = true,
                        IsStroke = true,
                        StrokeWidth = 3
                    };
                    canvas.DrawRoundRect(150, 1295, 162, 60, 10, 10, borderPaint);
                }
            }

            {
                using var textPaint = new SKPaint
                {
                    Color = SKColors.White,
                    TextSize = 38,
                    IsAntialias = true,
                    Typeface = TitilliumWeb_Regular
                };
                canvas.DrawCenteredText("Generated by YukiChan", 100, 1477, textPaint, 700);
            }

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, 70);
            return data.ToArray();
        });
    }
}