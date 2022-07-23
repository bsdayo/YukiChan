using ArcaeaUnlimitedAPI.Lib;
using SkiaSharp;
using YukiChan.Modules.Arcaea.Models;

namespace YukiChan.Modules.Arcaea.Images;

internal static partial class ArcaeaImageGenerator
{
    internal static async Task<byte[]> Best30(ArcaeaBest30 best30, AuaClient client)
    {
        return await Task.Run(() =>
        {
            var imageInfo = new SKImageInfo(3400, 6600);
            using var surface = SKSurface.Create(imageInfo);
            var canvas = surface.Canvas;

            {
                // TODO: B30 Background
                using var paint = new SKPaint
                {
                    Color = SKColors.Gray
                };
                canvas.DrawRect(0, 0, 3400, 6600, paint);
            }

            {
                // 账号信息
                using var paint = new SKPaint
                {
                    Color = SKColors.Black,
                    TextSize = 128,
                    IsAntialias = true,
                    Typeface = FontBold
                };
                canvas.DrawText(best30.Name, 295, 205, paint);
            }

            {
                // 账号信息
                using var paint = new SKPaint
                {
                    Color = SKColors.Black,
                    TextSize = 62,
                    IsAntialias = true,
                    Typeface = FontBold
                };
                canvas.DrawText(
                    $"B30Avg / {best30.Best30Avg:0.0000}   " +
                    $"R10Avg / {best30.Recent10Avg:0.0000}   " +
                    "MaxPtt / <TODO>",
                    295, 315, paint);
            }

            for (var col = 0; col < 3; col++)
            for (var row = 0; row < 10; row++)
            {
                var index = col * 10 + row;

                if (index > best30.Records.Length - 1)
                    break;

                var record = best30.Records[index];

                var songCover = client
                    .GetSongCover(record.SongId, record.JacketOverride, record.Difficulty)
                    .Result;

                canvas.DrawMiniScoreCard(
                    100 + col * 1100, 765 + row * 400, record, songCover, index);
            }

            var data = surface
                .Snapshot()
                .Encode(SKEncodedImageFormat.Jpeg, 70)
                .ToArray();

            return data;
        });
    }

    internal static void DrawMiniScoreCard(this SKCanvas canvas, int x, int y,
        ArcaeaRecord record, byte[] songCover, int rank = 0)
    {
        var (colorLight, colorDark) = DifficultyColors[(int)record.Difficulty];

        {
            // 背景
            using var backgroundPaint = new SKPaint
            {
                Color = SKColors.White
            };
            canvas.DrawRoundRect(x, y, 1000, 320, 20, 20, backgroundPaint);
        }

        // 处理曲绘
        var originalSongCover = SKBitmap.Decode(songCover);
        var scaledSongCover = new SKBitmap(290, 290);
        originalSongCover.ScalePixels(scaledSongCover, SKFilterQuality.Medium);

        canvas.DrawBitmap(scaledSongCover, x + 15, y + 15);

        // 排名
        if (rank != 0)
        {
            using var textPaint = new SKPaint
            {
                TextSize = 45,
                IsAntialias = true,
                Typeface = FontBold
            };
            using var rectPaint = new SKPaint();
            textPaint.Color = SKColor.Parse(rank switch
            {
                3 => "#ffffff",
                _ => "#333333"
            });
            rectPaint.Color = SKColor.Parse(rank switch
            {
                1 => "#ffcc00",
                2 => "#c0c0c0",
                3 => "#a57c50",
                _ => "#dddddd"
            });

            canvas.DrawRoundRect(x + 320, y + 15, 665, 60, 10, 10, rectPaint);
            canvas.DrawText($"#{rank}", x + 895, y + 61, textPaint);
        }

        {
            // 难度条
            using var rectPaint = new SKPaint
            {
                Color = SKColor.Parse(colorDark)
            };
            using var textPaint = new SKPaint
            {
                Color = SKColor.Parse("#ffffff"),
                TextSize = 45,
                IsAntialias = true,
                Typeface = FontRegular,
                TextScaleX = rank != 0 ? 339 : 444
            };

            canvas.DrawRoundRect(x + 320, y + 15, rank != 0 ? 560 : 665, 60, 10, 10, rectPaint);
            canvas.DrawText(
                $"{record.Difficulty} {record.RatingText} [{record.Rating}]",
                x + 526, y + 61, textPaint);
        }

        {
            // 获得 ptt
            using var rectPaint = new SKPaint
            {
                Color = SKColor.Parse(colorLight)
            };
            using var textPaint = new SKPaint
            {
                Color = SKColor.Parse("#ffffff"),
                TextSize = 45,
                IsAntialias = true,
                Typeface = FontBold
            };
            canvas.DrawRoundRect(x + 320, y + 15, 191, 60, 10, 10, rectPaint);
            canvas.DrawText($"{record.Potential:0.0000}", x + 335, y + 61, textPaint);
        }

        {
            // 曲名
            using var textPaint = new SKPaint
            {
                Color = SKColor.Parse("#333333"),
                TextSize = 60,
                IsAntialias = true,
                Typeface = FontBold
            };


            var originalLength = textPaint.MeasureText(record.Name);
            if (originalLength > 635)
                textPaint.TextScaleX = 635 / originalLength;

            canvas.DrawText(record.Name, x + 335, y + 136, textPaint);
        }

        {
            // 得分
            // 若理论值则绘制蓝色阴影
            if (record.ShinyPureCount == record.PureCount &&
                record.FarCount == 0 &&
                record.LostCount == 0)
            {
                using var maxPaint = new SKPaint
                {
                    Color = SKColor.Parse("#7fdfff"),
                    TextSize = 97,
                    IsAntialias = true,
                    Typeface = FontRegular,
                    TextScaleX = 635
                };
                canvas.DrawText(record.Score.FormatScore(), x + 340, y + 241, maxPaint);
            }

            using var textPaint = new SKPaint
            {
                Color = SKColor.Parse("#333333"),
                TextSize = 97,
                IsAntialias = true,
                Typeface = FontRegular,
                TextScaleX = 635
            };
            canvas.DrawText(record.Score.FormatScore(), x + 335, y + 236, textPaint);
        }

        {
            // Pure/Far/Lost 信息
            using var textPaint = new SKPaint
            {
                Color = SKColor.Parse("#333333"),
                TextSize = 97,
                IsAntialias = true,
                Typeface = FontRegular,
                TextScaleX = 635
            };
            canvas.DrawText(
                $"Pure / {record.PureCount} ({record.ShinyPureCount})   " +
                $"Far / {record.FarCount}   " +
                $"Lost / {record.LostCount}",
                x + 335, y + 296, textPaint);
        }
    }
}