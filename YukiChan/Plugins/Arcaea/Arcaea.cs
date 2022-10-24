using ArcaeaUnlimitedAPI.Lib;
using Flandre.Core.Attributes;
using Flandre.Core.Common;

namespace YukiChan.Plugins.Arcaea;

[Plugin("Arcaea")]
public partial class ArcaeaPlugin : Plugin
{
    private ArcaeaPluginConfig _config;

    private readonly AuaClient _auaClient;

    public ArcaeaPlugin(ArcaeaPluginConfig config)
    {
        _config = config;
        _auaClient = new AuaClient
        {
            ApiUrl = config.AuaApiUrl,
            UserAgent = config.AuaToken,
            Token = config.AuaToken,
            Timeout = config.AuaTimeout
        }.Initialize();
    }
}