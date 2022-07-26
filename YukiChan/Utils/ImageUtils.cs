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
}