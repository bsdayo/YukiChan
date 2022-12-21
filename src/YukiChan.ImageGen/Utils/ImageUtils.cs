using SkiaSharp;

namespace YukiChan.ImageGen.Utils;

public static class ImageUtils
{
    public static void DrawLimitedText(this SKCanvas canvas, string text,
        float x, float y, SKPaint paint, float widthLimit)
    {
        var originalWidth = paint.MeasureText(text);
        paint.TextScaleX = originalWidth > widthLimit ? widthLimit / originalWidth : 1;
        canvas.DrawText(text, x, y, paint);
    }

    public static void DrawCenteredText(this SKCanvas canvas, string text,
        float x, float y, SKPaint paint, float width)
    {
        var originalWidth = paint.MeasureText(text);

        if (originalWidth > width)
            paint.TextScaleX = width / originalWidth;
        else
            x += (width - originalWidth) / 2;

        canvas.DrawText(text, x, y, paint);
    }
}