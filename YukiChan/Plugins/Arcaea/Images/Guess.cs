using SkiaSharp;

namespace YukiChan.Plugins.Arcaea.Images;

public static class ArcaeaGuessImageGenerator
{
    public static byte[] Generate(byte[] songCover, ArcaeaGuessMode mode)
    {
        using var coverBitmap = SKBitmap.Decode(songCover);

        var width = mode switch
        {
            ArcaeaGuessMode.Easy => 100,
            ArcaeaGuessMode.Normal => 64,
            ArcaeaGuessMode.Hard => 40,
            ArcaeaGuessMode.Flash => 100,
            ArcaeaGuessMode.GrayScale => 100,
            ArcaeaGuessMode.Invert => 100,
            _ => 64
        };

        var imageInfo = new SKImageInfo(width, width);
        using var surface = SKSurface.Create(imageInfo);
        using var canvas = surface.Canvas;

        using var paint = new SKPaint();

        if (mode == ArcaeaGuessMode.GrayScale)
            paint.ColorFilter = SKColorFilter.CreateColorMatrix(new[]
            {
                0.2126f, 0.7152f, 0.0722f, 0f, 0f,
                0.2126f, 0.7152f, 0.0722f, 0f, 0f,
                0.2126f, 0.7152f, 0.0722f, 0f, 0f,
                0.0000f, 0.0000f, 0.0000f, 1f, 0f
            });

        if (mode == ArcaeaGuessMode.Invert)
            for (var i = 0; i < coverBitmap.Width; i++)
            for (var j = 0; j < coverBitmap.Height; j++)
            {
                var color = coverBitmap.GetPixel(i, j);
                coverBitmap.SetPixel(i, j, new SKColor(
                    (byte)(255 - color.Red),
                    (byte)(255 - color.Green),
                    (byte)(255 - color.Blue)
                ));
            }

        canvas.DrawBitmap(coverBitmap,
            new Random().Next(imageInfo.Width - coverBitmap.Width, 0),
            new Random().Next(imageInfo.Height - coverBitmap.Height, 0),
            paint);

        return surface
            .Snapshot()
            .Encode(SKEncodedImageFormat.Jpeg, 70)
            .ToArray();
    }
}