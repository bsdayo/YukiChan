using Microsoft.EntityFrameworkCore;
using YukiChan.Shared.Models.Arcaea;
using YukiChan.Shared.Utils;

namespace YukiChan.Tools.Utils;

public sealed class ArcaeaFakeData
{
    private readonly ArcaeaSongDbContext _songDb;

    public ArcaeaFakeData(ArcaeaSongDbContext songDb) => _songDb = songDb;

    private ArcaeaRecord GetRecord(ArcaeaSongDbChart chart)
    {
        var farCount = Random.Shared.Next(100);
        var lostCount = Random.Shared.Next(40);
        var pureCount = chart.Note - farCount - lostCount;
        var score = 9_000_000 + Random.Shared.Next(1_000_000) + chart.Note;
        return new ArcaeaRecord
        {
            Name = chart.NameEn,
            SongId = chart.SongId,
            Potential = ArcaeaSharedUtils.CalculatePotential(chart.Rating / 10d, score),
            Rating = chart.Rating / 10d,
            Difficulty = (ArcaeaDifficulty)chart.RatingClass,
            Score = score,
            ShinyPureCount = pureCount - Random.Shared.Next(100),
            PureCount = pureCount,
            FarCount = farCount,
            LostCount = lostCount,
            ClearType = (ArcaeaClearType)Random.Shared.Next(1, 5),
            Grade = ArcaeaSharedUtils.GetGrade(score),
            RecollectionRate = Random.Shared.Next(100),
            JacketOverride = chart.JacketOverride,
            PlayTime = DateTime.UtcNow.AddDays(-Random.Shared.Next(365))
        };
    }

    public ArcaeaRecord Record(string songId = "equilibrium")
    {
        var chart = _songDb.Charts
            .AsNoTracking()
            .First(chart => chart.SongId == songId && chart.RatingClass == 2);
        return GetRecord(chart);
    }

    public ArcaeaBest30 Best30()
    {
        var allRecords = _songDb.Charts
            .AsNoTracking()
            .OrderByDescending(chart => chart.Rating)
            .Take(40)
            .ToList()
            .Select(GetRecord)
            .OrderByDescending(record => record.Potential)
            .ToArray();

        return new ArcaeaBest30
        {
            User = User(),
            Recent10Avg = 11.4514,
            Best30Avg = 19.1981,
            Records = allRecords[..30],
            OverflowRecords = allRecords[30..]
        };
    }

    public ArcaeaUser User()
    {
        return new ArcaeaUser
        {
            Name = "FakeUser",
            Code = "007355608",
            Potential = 99.99,
            JoinTime = DateTime.UnixEpoch
        };
    }
}