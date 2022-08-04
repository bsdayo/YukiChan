using SkiaSharp;

namespace YukiChan.Modules.Arcaea.Images;

public static class ArcaeaGuessImageGenerator
{
    public static byte[] Normal(byte[] songCover, ArcaeaGuessMode mode)
    {
        using var coverBitmap = SKBitmap.Decode(songCover);

        var width = mode switch
        {
            ArcaeaGuessMode.Easy => 100,
            ArcaeaGuessMode.Normal => 64,
            ArcaeaGuessMode.Hard => 40,
            _ => 64
        };

        var imageInfo = new SKImageInfo(width, width);
        using var surface = SKSurface.Create(imageInfo);
        using var canvas = surface.Canvas;

        canvas.DrawBitmap(coverBitmap,
            new Random().Next(imageInfo.Width - coverBitmap.Width, 0),
            new Random().Next(imageInfo.Height - coverBitmap.Height, 0));

        return surface
            .Snapshot()
            .Encode(SKEncodedImageFormat.Jpeg, 70)
            .ToArray();
    }
}