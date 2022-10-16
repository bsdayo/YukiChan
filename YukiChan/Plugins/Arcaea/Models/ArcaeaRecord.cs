using ArcaeaUnlimitedAPI.Lib.Models;

// ReSharper disable InconsistentNaming

#pragma warning disable CS8618

namespace YukiChan.Plugins.Arcaea.Models;

public class ArcaeaRecord
{
    public string Name { get; init; }

    public string SongId { get; init; }

    public double Potential { get; init; }

    public string Rating { get; init; }

    public string RatingText { get; init; }

    public ArcaeaDifficulty Difficulty { get; init; }

    public int Score { get; init; }

    public int ShinyPureCount { get; init; }

    public int PureCount { get; init; }

    public int FarCount { get; init; }

    public int LostCount { get; init; }

    public ArcaeaClearType ClearType { get; init; }

    public ArcaeaGrade Grade { get; init; }

    public int RecollectionRate { get; init; }

    public bool JacketOverride { get; init; }

    public long TimePlayed { get; init; }

    public static ArcaeaRecord FromAua(AuaRecord record, AuaChartInfo chartInfo)
    {
        return new ArcaeaRecord
        {
            Name = chartInfo.NameEn,
            SongId = record.SongId,
            Potential = record.Rating,
            Difficulty = (ArcaeaDifficulty)record.Difficulty,
            Rating = ((double)chartInfo.Rating / 10).ToString("0.0"),
            RatingText = chartInfo.Rating.GetDifficulty(),
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
        var chart = ArcaeaSongDatabase.GetChartsById(record.SongId)[record.Difficulty];
        return new ArcaeaRecord
        {
            Name = chart.NameEn,
            SongId = record.SongId,
            Potential = ArcaeaUtils.CalculatePotential((double)chart.Rating / 10, record.Score),
            Difficulty = (ArcaeaDifficulty)record.Difficulty,
            Rating = ((double)chart.Rating / 10).ToString("0.0"),
            RatingText = chart.Rating.GetDifficulty(),
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
}

public enum ArcaeaClearType
{
    TrackLost = 0,
    NormalClear = 1,
    FullRecall = 2,
    PureMemory = 3,
    EasyClear = 4,
    HardClear = 5
}

public enum ArcaeaGrade
{
    D,
    C,
    B,
    A,
    AA,
    EX,
    EXP
}