#pragma warning disable CS8618

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace YukiChan.Shared.Models.Arcaea;

public sealed class ArcaeaBest30
{
    public ArcaeaUser User { get; set; }

    public double Recent10Avg { get; set; }

    public double Best30Avg { get; set; }

    public ArcaeaRecord[] Records { get; set; }

    public ArcaeaRecord[]? OverflowRecords { get; set; }

    public bool HasOverflow => OverflowRecords is not null;
}