using YukiChan.Shared.Models.Arcaea;

namespace YukiChan.Shared.Data.Console.Arcaea;

public sealed class ArcaeaBestResponse
{
    public required ArcaeaUser User { get; set; }

    public required ArcaeaRecord BestRecord { get; set; }
}