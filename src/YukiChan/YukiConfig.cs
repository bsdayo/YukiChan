using Flandre.Framework;
using Flandre.Plugins.BaiduTranslate;
using Flandre.Plugins.HttpCat;
using Flandre.Plugins.WolframAlpha;
using YukiChan.Plugins;
using YukiChan.Plugins.Arcaea;
using YukiChan.Plugins.SandBox;

namespace YukiChan;

public class YukiConfig
{
    public bool EnableDebugLog { get; set; } = true;

    public string[] QqGuildAllowedChannels { get; set; } = Array.Empty<string>();

    public string[] NoWarnQqGroups { get; set; } = Array.Empty<string>();

    public FlandreAppConfig App { get; set; } = new();

    public YukiPluginsConfig Plugins { get; set; } = new();
}

public class YukiPluginsConfig
{
    public ArcaeaPluginConfig Arcaea { get; set; } = new();

    public HttpCatPluginConfig HttpCat { get; set; } = new();

    public WolframAlphaPluginConfig WolframAlpha { get; set; } = new();

    public BaiduTranslatePluginConfig BaiduTranslate { get; set; } = new();

    public SandBoxPluginConfig SandBox { get; set; } = new();

    public AutoAcceptPluginConfig AutoAccept { get; set; } = new();

    public GosenPluginConfig Gosen { get; set; } = new();
}