using ArcaeaUnlimitedAPI.Lib.Models;

#pragma warning disable CS8618

namespace YukiChan.Modules.Arcaea.Models;

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

    public byte RecollectionRate { get; init; }

    public bool JacketOverride { get; init; }

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
            //
            ShinyPureCount = record.ShinyPerfectCount,
            PureCount = record.PerfectCount,
            FarCount = record.NearCount,
            LostCount = record.MissCount,
            RecollectionRate = (byte)record.Health!,
            //
            JacketOverride = chartInfo.JacketOverride
        };
    }
}