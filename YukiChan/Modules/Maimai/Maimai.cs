using YukiChan.Core;
using YukiChan.Utils;

namespace YukiChan.Modules.Maimai;

[Module("Maimai",
    Command = "mai",
    Description = "maimai 相关功能",
    Version = "0.1.0")]
public partial class MaimaiModule : ModuleBase
{
    private static readonly DivingFishClient DivingFishClient = new();

    private static readonly ModuleLogger Logger = new("Maimai");
}