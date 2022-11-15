using SkiaSharp;
using YukiChan.Utils;

// ReSharper disable InconsistentNaming

namespace YukiChan.Plugins.Arcaea.Images;

public static partial class ArcaeaImageGenerator
{
    private static SKTypeface TitilliumWeb_Regular =>
        SKTypeface.FromFile($"{YukiDir.Assets}/fonts/TitilliumWeb-Regular.ttf");

    private static SKTypeface TitilliumWeb_SemiBold =>
        SKTypeface.FromFile($"{YukiDir.Assets}/fonts/TitilliumWeb-SemiBold.ttf");

    private static SKTypeface GeoSans =>
        SKTypeface.FromFile($"{YukiDir.Assets}/fonts/GeosansLight.ttf");

    private static readonly List<(
        string ColorLight, string ColorDark,
        string ColorBorderLight, string ColorBorderDark,
        string ColorInnerLight, string ColorInnerDark)> DifficultyColors = new()
    {
        ("#51a9c8", "#4188a1", "#71c9e8", "#61a8c1", "#3189a8", "#216881"), // Past
        ("#a8c96b", "#87a256", "#c8e98b", "#a7c276", "#88a94b", "#678236"), // Present
        ("#8a4876", "#6f3a5f", "#aa6896", "#8f5a7f", "#6a2856", "#4f1a3f"), // Future
        ("#bb2b43", "#962336", "#db4b63", "#b64356", "#9b0b23", "#760316") // Beyond
    };
}