using YukiChan.Shared.Models.Arcaea;

namespace YukiChan.Shared.Data.Console.Arcaea;

public sealed class ArcaeaRecentResponse
{
    public required ArcaeaUser User { get; set; }

    public required ArcaeaRecord RecentRecord { get; set; }
}