using ArcaeaUnlimitedAPI.Lib.Responses;

#pragma warning disable CS8618

namespace YukiChan.Modules.Arcaea.Models;

public class ArcaeaBest30
{
    public string Name { get; private init; }

    public string Id { get; private init; }

    public string Potential { get; private init; }

    public double Recent10Avg { get; private init; }

    public double Best30Avg { get; private init; }

    public ArcaeaRecord[] Records { get; private init; }

    public ArcaeaRecord[]? OverflowRecords { get; private init; }

    public bool HasOverflow => OverflowRecords is not null;

    public static ArcaeaBest30 FromAua(AuaUserBest30Content content)
    {
        return new ArcaeaBest30
        {
            Name = content.AccountInfo.Name,
            Id = content.AccountInfo.Code,
            Potential = ((double)content.AccountInfo.Rating / 100).ToString("0.00"),

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