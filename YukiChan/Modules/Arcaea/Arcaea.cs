using ArcaeaUnlimitedAPI.Lib;
using YukiChan.Core;
using YukiChan.Utils;

namespace YukiChan.Modules.Arcaea;

[Module("Arcaea",
    Command = "a",
    Description = "Arcaea 相关功能",
    Version = "1.3.0")]
public partial class ArcaeaModule : ModuleBase
{
    private static readonly AuaClient AuaClient = new AuaClient
    {
        ApiUrl = Global.YukiConfig.Arcaea.AuaApiUrl,
        UserAgent = Global.YukiConfig.Arcaea.AuaToken,
        Timeout = Global.YukiConfig.Arcaea.AuaTimeout
    }.Initialize();

    private static readonly AlaClient AlaClient = new()
    {
        Token = Global.YukiConfig.Arcaea.AlaToken,
        Timeout = Global.YukiConfig.Arcaea.AlaTimeout
    };

    private static readonly ModuleLogger Logger = new("Arcaea");
}