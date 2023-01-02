using System.Text.Json.Serialization;

#pragma warning disable CS8618

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace YukiChan.Shared.Arcaea.Models;

public sealed class ArcaeaBest30
{
    public ArcaeaUser User { get; set; }

    public double Recent10Avg { get; set; }

    public double Best30Avg { get; set; }

    public ArcaeaRecord[] Records { get; set; }

    public ArcaeaRecord[]? OverflowRecords { get; set; }

    [JsonIgnore]
    public bool HasOverflow => OverflowRecords is not null;
}