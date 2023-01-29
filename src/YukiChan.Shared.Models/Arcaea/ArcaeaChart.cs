using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618

namespace YukiChan.Shared.Models.Arcaea;

public class ArcaeaChart
{
    public ArcaeaDifficulty Difficulty { get; init; }

    public string NameEn { get; init; }

    public string NameJp { get; init; }

    public string Artist { get; init; }

    public string Bpm { get; init; }

    public double BpmBase { get; init; }

    public int Time { get; init; }

    public int Side { get; init; }

    public bool WorldUnlock { get; init; }

    public bool RemoteDownload { get; init; }

    public string Background { get; init; }

    public long Date { get; init; }

    public string Version { get; init; }

    public double Rating { get; init; }

    public int Note { get; init; }

    public string ChartDesigner { get; init; }

    public string JacketDesigner { get; init; }

    public bool JacketOverride { get; init; }

    public bool AudioOverride { get; init; }
}