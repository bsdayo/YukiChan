using SkiaSharp;
using YukiChan.Utils;

namespace YukiChan.Plugins.Arcaea.Images;

public static partial class ArcaeaImageGenerator
{
    private static SKTypeface FontRegular =>
        SKTypeface.FromFile($"{YukiDir.Assets}/fonts/TitilliumWeb-Regular.ttf");

    private static SKTypeface FontBold =>
        SKTypeface.FromFile($"{YukiDir.Assets}/fonts/TitilliumWeb-SemiBold.ttf");

    private static SKTypeface GeoSans =>
        SKTypeface.FromFile($"{YukiDir.Assets}/fonts/GeosansLight.ttf");

    // 避免使用枚举类型作为 Dictionary 的键，以免隐式装箱导致不必要的性能损耗
    // 显式转换为 int 即可避免该问题 
    private static readonly Dictionary<int, (string, string, string, string, string, string)> DifficultyColors = new()
    {
        // { DifficultyIndex, (ColorLight, ColorDark, ColorBorderLight, ColorBorderDark, ColorInnerLight, ColorInnerDark) }
        { 0, ("#51a9c8", "#4188a1", "#71c9e8", "#61a8c1", "#3189a8", "#216881") }, // Past
        { 1, ("#a8c96b", "#87a256", "#c8e98b", "#a7c276", "#88a94b", "#678236") }, // Present
        { 2, ("#8a4876", "#6f3a5f", "#aa6896", "#8f5a7f", "#6a2856", "#4f1a3f") }, // Future
        { 3, ("#bb2b43", "#962336", "#db4b63", "#b64356", "#9b0b23", "#760316") } // Beyond
    };
}