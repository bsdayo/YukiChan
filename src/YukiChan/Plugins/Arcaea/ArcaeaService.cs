using ArcaeaUnlimitedAPI.Lib;

namespace YukiChan.Plugins.Arcaea;

public class ArcaeaService
{
    public AuaClient AuaClient { get; }
    
    public AlaClient AlaClient { get; }

    public ArcaeaService(ArcaeaPluginConfig config)
    {
        AuaClient = new AuaClient
        {
            ApiUrl = config.AuaApiUrl,
            UserAgent = config.AuaToken,
            Token = config.AuaToken,
            Timeout = config.AuaTimeout
        }.Initialize();

        AlaClient = new AlaClient
        {
            Token = config.AlaToken,
            Timeout = config.AlaTimeout
        };
    }
}