using Flandre.Core;
using Flandre.Plugins.HttpCat;
using YukiChan.Plugins.Arcaea;

namespace YukiChan;

public class YukiConfig
{
    public bool EnableDebugLog { get; set; } = true;

    public string[] QqGuildAllowedChannels { get; set; } = Array.Empty<string>();

    public FlandreAppConfig App { get; set; } = new();

    public YukiPluginsConfig Plugins { get; set; } = new();
}

public class YukiPluginsConfig
{
    public ArcaeaPluginConfig Arcaea { get; set; } = new();

    public HttpCatPluginConfig HttpCat { get; set; } = new();
}