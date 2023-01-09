namespace YukiChan.Shared;

public static class YukiDir
{
    // Global
    public static readonly string Root = AppDomain.CurrentDomain.BaseDirectory;
    public static readonly string Configs = Path.Combine(Root, "configs");
    public static readonly string Logs = Path.Combine(Root, "logs");
    public static readonly string Cache = Path.Combine(Root, "cache");
    public static readonly string Assets = Path.Combine(Root, "assets");
    public static readonly string Data = Path.Combine(Root, "data");
    public static readonly string Databases = Path.Combine(Root, "databases");

    public static readonly string Fonts = Path.Combine(Assets, "fonts");

    // Plugin Specific
    // Arcaea
    public static readonly string ArcaeaAssets = Path.Combine(Assets, "arcaea");
    public static readonly string ArcaeaCache = Path.Combine(Cache, "arcaea");
    public static readonly string ArcaeaData = Path.Combine(Data, "arcaea");

    // Images
    public static readonly string CatAssets = Path.Combine(Assets, "cats");
    public static readonly string CapooAssets = Path.Combine(Assets, "capoo");

    // HttpCat
    public static readonly string HttpCatCache = Path.Combine(Cache, "httpcat");

    public static void CreateIfNotExists(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    static YukiDir()
    {
        var dirs = new[]
        {
            Configs, Logs, Cache, Assets, Data, Databases,

            // Logs
            $"{Logs}/yuki", $"{Logs}/konata",

            // Arcaea
            $"{ArcaeaAssets}/audio-preview", $"{ArcaeaAssets}/aff", $"{ArcaeaAssets}/images",
            $"{ArcaeaCache}/song", $"{ArcaeaCache}/char", $"{ArcaeaCache}/preview",
            $"{ArcaeaCache}/single-dynamic-bg-v1", // $"{ArcaeaCache}/single-dynamic-bg-v2",
            $"{ArcaeaData}/best30",

            // Images
            $"{Assets}/cats", $"{Assets}/capoo",

            // HttpCat
            HttpCatCache
        };

        foreach (var dir in dirs)
            CreateIfNotExists(dir);
    }
}