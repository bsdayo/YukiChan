using Microsoft.Extensions.Logging;
using SkiaSharp;
using YukiChan.Client.Console;
using YukiChan.Core;
using YukiChan.ImageGen.Utils;
using YukiChan.Shared.Models.Arcaea;

namespace YukiChan.ImageGen.Arcaea;

public partial class ArcaeaImageGenerator
{
    public async Task<byte[]> SingleV2(ArcaeaUser user, ArcaeaRecord record, YukiConsoleClient? client,
        ArcaeaUserPreferences pref, ILogger? logger = null)
    {
        var imageInfo = new SKImageInfo(2000, 1000);
        using var surface = SKSurface.Create(imageInfo);
        using var canvas = surface.Canvas;

        if (pref.SingleDynamicBackground)
        {
            var background = await GetSingleV2Background(record, client, logger);
            canvas.DrawBitmap(SKBitmap.Decode(background), 0, 0);
        }

        // TODO: Implement Single V2

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 70);
        return data.ToArray();
    }

    public async Task<byte[]> GetSingleV2Background(ArcaeaRecord record, YukiConsoleClient? client, ILogger? logger)
    {
        var path = record.JacketOverride
            ? $"{YukiDir.ArcaeaCache}/single-dynamic-bg-v2/{record.SongId}-{record.Difficulty.ToString().ToLower()}.jpg"
            : $"{YukiDir.ArcaeaCache}/single-dynamic-bg-v2/{record.SongId}.jpg";

        if (File.Exists(path))
            return await File.ReadAllBytesAsync(path);

        var coverBitmap = SKBitmap.Decode(
            await ArcaeaImageUtils.GetSongCover(client, record.SongId, record.JacketOverride, record.Difficulty, false,
                logger))!;
        using var scaledCoverBitmap = new SKBitmap(2000, 2000);
        coverBitmap.ScalePixels(scaledCoverBitmap, SKFilterQuality.Low);
        coverBitmap.Dispose();

        var imageInfo = new SKImageInfo(2000, 1000);
        using var surface = SKSurface.Create(imageInfo);
        using var canvas = surface.Canvas;

        using var paint = new SKPaint
        {
            ImageFilter = SKImageFilter.CreateBlur(15, 15)
        };
        canvas.DrawBitmap(scaledCoverBitmap, 0, -500, paint);

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 70);

        var result = data.ToArray();

        await File.WriteAllBytesAsync(path, result);

        return result;
    }
}