using SkiaSharp;

namespace YukiChan.Utils;

public static class ImageUtils
{
    public static void DrawLimitedText(this SKCanvas canvas, string text,
        float x, float y, SKPaint paint, float widthLimit)
    {
        var originalWidth = paint.MeasureText(text);

        if (originalWidth > widthLimit)
            paint.TextScaleX = widthLimit / originalWidth;

        canvas.DrawText(text, x, y, paint);
    }

    public static void DrawCenteredText(this SKCanvas canvas, string text,
        float x, float y, SKPaint paint, float widthLimit)
    {
        var originalWidth = paint.MeasureText(text);

        if (originalWidth > widthLimit)
            paint.TextScaleX = widthLimit / originalWidth;
        else
            x += (widthLimit - originalWidth) / 2;

        canvas.DrawText(text, x, y, paint);
    }
}