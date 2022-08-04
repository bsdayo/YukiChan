using ArcaeaUnlimitedAPI.Lib.Responses;

#pragma warning disable CS8618

namespace YukiChan.Modules.Arcaea.Models;

public class ArcaeaBest30
{
    public ArcaeaUser User { get; private init; }

    public double Recent10Avg { get; private init; }

    public double Best30Avg { get; private init; }

    public ArcaeaRecord[] Records { get; private init; }

    public ArcaeaRecord[]? OverflowRecords { get; private init; }

    public bool HasOverflow => OverflowRecords is not null;

    public static ArcaeaBest30 FromAua(AuaUserBest30Content content)
    {
        return new ArcaeaBest30
        {
            User = ArcaeaUser.FromAua(content.AccountInfo),

            Recent10Avg = content.Recent10Avg,
            Best30Avg = content.Best30Avg,

            Records = content.Best30List.Select((record, i)
                    => ArcaeaRecord.FromAua(record, content.Best30SongInfo![i]))
                .ToArray(),

            OverflowRecords = content.Best30Overflow is null
                ? Array.Empty<ArcaeaRecord>()
                : content.Best30Overflow.Select((record, i)
                        => ArcaeaRecord.FromAua(record, content.Best30OverflowSongInfo![i]))
                    .ToArray()
        };
    }
}