using ArcaeaUnlimitedAPI.Lib.Models;
using YukiChan.Shared.Arcaea.Models;
using YukiChan.Shared.Utils;

namespace YukiChan.Shared.Arcaea.Factories;

public static class ArcaeaRecordFactory
{
    public static ArcaeaRecord FromAua(AuaRecord record, AuaChartInfo chartInfo)
    {
        return new ArcaeaRecord
        {
            Name = chartInfo.NameEn,
            SongId = record.SongId,
            Potential = record.Rating,
            Difficulty = (ArcaeaDifficulty)record.Difficulty,
            Rating = ((double)chartInfo.Rating / 10).ToString("0.0"),
            RatingText = chartInfo.Rating.GetRatingText(),
            Score = record.Score,
            ClearType = (ArcaeaClearType)record.ClearType!,
            Grade = ArcaeaUtils.GetGrade(record.Score),
            //
            ShinyPureCount = record.ShinyPerfectCount,
            PureCount = record.PerfectCount,
            FarCount = record.NearCount,
            LostCount = record.MissCount,
            RecollectionRate = record.Health!.Value,
            //
            JacketOverride = chartInfo.JacketOverride,
            TimePlayed = record.TimePlayed
        };
    }

    public static ArcaeaRecord FromAla(AlaRecord record)
    {
        var chart = ArcaeaSongDatabase.Default.GetChartsById(record.SongId).Result[record.Difficulty];
        return new ArcaeaRecord
        {
            Name = chart.NameEn,
            SongId = record.SongId,
            Potential = ArcaeaUtils.CalculatePotential((double)chart.Rating / 10, record.Score),
            Difficulty = (ArcaeaDifficulty)record.Difficulty,
            Rating = ((double)chart.Rating / 10).ToString("0.0"),
            RatingText = chart.Rating.GetRatingText(),
            Score = record.Score,
            ClearType = record.RecollectionRate >= 70 ? ArcaeaClearType.NormalClear : ArcaeaClearType.TrackLost,
            Grade = ArcaeaUtils.GetGrade(record.Score),
            //
            ShinyPureCount = record.ShinyPureCount,
            PureCount = record.PureCount,
            FarCount = record.FarCount,
            LostCount = record.LostCount,
            RecollectionRate = record.RecollectionRate,
            //
            JacketOverride = chart.JacketOverride,
            TimePlayed = record.TimePlayed
        };
    }

    public static ArcaeaRecord GenerateFake(ArcaeaSongDbChart chart)
    {
        return new ArcaeaRecord
        {
            Name = chart.NameEn,
            SongId = chart.SongId,
            Potential = chart.Rating / 10d + 2,
            Rating = (chart.Rating / 10d).ToString("0.0"),
            RatingText = chart.Rating.GetRatingText(),
            Difficulty = (ArcaeaDifficulty)chart.RatingClass,
            Score = 10_000_000 + chart.Note,
            ShinyPureCount = chart.Note,
            PureCount = chart.Note,
            FarCount = 0,
            LostCount = 0,
            ClearType = ArcaeaClearType.PureMemory,
            Grade = ArcaeaGrade.EXP,
            RecollectionRate = 100,
            JacketOverride = chart.JacketOverride,
            TimePlayed = chart.Date * 1000
        };
    }
}