namespace YukiChan.Utils;

public static class YukiDir
{
    // Global

    public static readonly string Root = AppDomain.CurrentDomain.BaseDirectory;

    public static readonly string Configs = Path.Combine(Root, "configs");

    public static readonly string Logs = Path.Combine(Root, "logs");

    public static readonly string Cache = Path.Combine(Root, "cache");

    public static readonly string Assets = Path.Combine(Root, "assets");

    public static readonly string Databases = Path.Combine(Root, "databases");

    // Plugin Specific

    public static readonly string ArcaeaAssets = Path.Combine(Assets, "arcaea");

    public static readonly string ArcaeaCache = Path.Combine(Cache, "arcaea");

    public static void CreateIfNotExists(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    public static void EnsureExistence()
    {
        var dirs = new[]
        {
            Configs, Logs, Cache, Assets, Databases,

            // Logs
            $"{Logs}/yuki", $"{Logs}/konata",

            // Arcaea
            $"{ArcaeaAssets}/audio-preview", $"{ArcaeaAssets}/aff", $"{ArcaeaAssets}/images",
            $"{ArcaeaCache}/song", $"{ArcaeaCache}/preview",

            // Images
            $"{Assets}/cats", $"{Assets}/capoo"
        };

        foreach (var dir in dirs)
            CreateIfNotExists(dir);
    }
}