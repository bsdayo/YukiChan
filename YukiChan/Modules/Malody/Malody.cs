using YukiChan.Core;
using YukiChan.Utils;

namespace YukiChan.Modules.Malody;

[Module("Malody",
    Command = "ma",
    Description = "Malody 相关功能",
    Version = "1.0.0")]
public partial class MalodyModule : ModuleBase
{
    private static readonly MalodyInfoQuery.Core.Malody MalodyQuery = new(
        Global.YukiConfig.Malody.Account, Global.YukiConfig.Malody.Password, Global.YukiConfig.Malody.BaseUrl);

    private static readonly ModuleLogger Logger = new("Malody");
}