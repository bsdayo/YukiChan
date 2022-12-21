using ArcaeaUnlimitedAPI.Lib.Responses;
using YukiChan.Shared.Arcaea.Models;

namespace YukiChan.Shared.Arcaea.Factories;

public static class ArcaeaBest30Factory
{
    public static ArcaeaBest30 FromAua(AuaUserBest30Content content)
    {
        return new ArcaeaBest30
        {
            User = ArcaeaUserFactory.FromAua(content.AccountInfo),

            Recent10Avg = content.Recent10Avg,
            Best30Avg = content.Best30Avg,

            Records = content.Best30List.Select((record, i)
                    => ArcaeaRecordFactory.FromAua(record, content.Best30SongInfo![i]))
                .ToArray(),

            OverflowRecords = content.Best30Overflow is null
                ? Array.Empty<ArcaeaRecord>()
                : content.Best30Overflow.Select((record, i)
                        => ArcaeaRecordFactory.FromAua(record, content.Best30OverflowSongInfo![i]))
                    .ToArray()
        };
    }

    public static ArcaeaBest30 FromAla(AlaUser user, AlaRecord[] alaRecords, string usercode)
    {
        var best30 = new ArcaeaBest30
        {
            User = ArcaeaUserFactory.FromAla(user, usercode),
            Recent10Avg = 0,
            OverflowRecords = null
        };
        double totalPtt = 0;
        var records = new List<ArcaeaRecord>();
        foreach (var record in alaRecords)
        {
            var rec = ArcaeaRecordFactory.FromAla(record);
            records.Add(rec);
            totalPtt += rec.Potential;
        }

        best30.Records = records.ToArray();
        best30.Best30Avg = totalPtt / 30;

        return best30;
    }

    public static ArcaeaBest30 GenerateFake()
    {
        var allRecords = ArcaeaSongDatabase.Default
            .GetAllCharts().GetAwaiter().GetResult()
            .OrderByDescending(chart => chart.Rating)
            .Take(40)
            .Select(ArcaeaRecordFactory.GenerateFake)
            .ToArray();

        return new ArcaeaBest30
        {
            User = ArcaeaUserFactory.GenerateFake(),
            Recent10Avg = 11.4514,
            Best30Avg = 19.1981,
            Records = allRecords[..30],
            OverflowRecords = allRecords[30..]
        };
    }
}