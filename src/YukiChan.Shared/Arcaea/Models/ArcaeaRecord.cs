using ArcaeaUnlimitedAPI.Lib.Models;

// ReSharper disable InconsistentNaming

#pragma warning disable CS8618

namespace YukiChan.Shared.Arcaea.Models;

public sealed class ArcaeaRecord
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
}