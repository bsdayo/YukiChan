using ArcaeaUnlimitedAPI.Lib;
using YukiChan.Attributes;
using YukiChan.Core;
using YukiChan.Models;

namespace YukiChan.Modules.Arcaea;

[Module("Arcaea",
    Command = "a",
    Description = "Arcaea 相关功能",
    Version = "1.0.0")]
public partial class ArcaeaModule : ModuleBase
{
    private static AuaClient _auaClient = new AuaClient
    {
        ApiUrl = Global.YukiConfig.Arcaea.AuaApiUrl,
        UserAgent = Global.YukiConfig.Arcaea.UserAgent,
        Timeout = Global.YukiConfig.Arcaea.Timeout
    }.Initialize();
}