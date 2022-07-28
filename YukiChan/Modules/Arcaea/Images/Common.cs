using SkiaSharp;

namespace YukiChan.Modules.Arcaea.Images;

internal static partial class ArcaeaImageGenerator
{
    private static SKTypeface FontRegular =>
        SKTypeface.FromFile("Assets/Fonts/TitilliumWeb-Regular.ttf");

    private static SKTypeface FontBold =>
        SKTypeface.FromFile("Assets/Fonts/TitilliumWeb-SemiBold.ttf");

    // 避免使用枚举类型作为 Dictionary 的键，以免隐式装箱导致不必要的性能损耗
    // 显式转换为 int 即可避免该问题 
    private static readonly Dictionary<int, (string, string)> DifficultyColors = new()
    {
        // { DifficultyIndex, (ColorLight, ColorDark) }
        { 0, ("#51a9c8", "#4188a1") }, // Past
        { 1, ("#a8c96b", "#87a256") }, // Present
        { 2, ("#8a4876", "#6f3a5f") }, // Future
        { 3, ("#bb2b43", "#962336") } // Beyond
    };
}