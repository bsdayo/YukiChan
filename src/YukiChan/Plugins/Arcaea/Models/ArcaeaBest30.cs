using ArcaeaUnlimitedAPI.Lib.Responses;

#pragma warning disable CS8618

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace YukiChan.Plugins.Arcaea.Models;

public class ArcaeaBest30
{
    public ArcaeaUser User { get; set; }

    public double Recent10Avg { get; set; }

    public double Best30Avg { get; set; }

    public ArcaeaRecord[] Records { get; set; }

    public ArcaeaRecord[]? OverflowRecords { get; set; }

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

    public static ArcaeaBest30 FromAla(AlaUser user, AlaRecord[] alaRecords, string usercode)
    {
        var best30 = new ArcaeaBest30
        {
            User = ArcaeaUser.FromAla(user, usercode),
            Recent10Avg = 0,
            OverflowRecords = null
        };
        double totalPtt = 0;
        var records = new List<ArcaeaRecord>();
        foreach (var record in alaRecords)
        {
            var rec = ArcaeaRecord.FromAla(record);
            records.Add(rec);
            totalPtt += rec.Potential;
        }

        best30.Records = records.ToArray();
        best30.Best30Avg = totalPtt / 30;

        return best30;
    }
}