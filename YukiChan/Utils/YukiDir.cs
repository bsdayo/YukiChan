namespace YukiChan.Utils;

public static class YukiDir
{
    public static readonly string Root = AppDomain.CurrentDomain.BaseDirectory;

    public static string Configs => Path.Combine(Root, "configs");

    public static void EnsureExistence()
    {
        if (!Directory.Exists(Configs))
            Directory.CreateDirectory(Configs);
    }
}