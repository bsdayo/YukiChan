using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618

namespace YukiChan.Shared.Models.Arcaea;

/// <remarks>
/// 主键是 song_id 和 rating_class，在 ArcaeaSongDbContext 中使用 Fluent API 定义
/// </remarks>
[Table("charts")]
public sealed class ArcaeaSongDbChart
{
    [Column("song_id")]
    public string SongId { get; init; }

    [Column("rating_class")]
    public int RatingClass { get; init; }
    
    [Column("set")]
    public string Set { get; init; }

    [Column("name_en")]
    public string NameEn { get; init; }

    [Column("name_jp")]
    public string NameJp { get; init; }

    [Column("artist")]
    public string Artist { get; init; }

    [Column("bpm")]
    public string Bpm { get; init; }

    [Column("bpm_base")]
    public double BpmBase { get; init; }

    [Column("time")]
    public int Time { get; init; }

    [Column("side")]
    public int Side { get; init; }

    [Column("world_unlock")]
    public bool WorldUnlock { get; init; }

    [Column("remote_download")]
    public bool RemoteDownload { get; init; }

    [Column("bg")]
    public string Bg { get; init; }

    [Column("date")]
    public long Date { get; init; }

    [Column("version")]
    public string Version { get; init; }

    [Column("difficulty")]
    public int Difficulty { get; init; }

    [Column("rating")]
    public int Rating { get; init; }

    [Column("note")]
    public int Note { get; init; }

    [Column("chart_designer")]
    public string ChartDesigner { get; init; }

    [Column("jacket_designer")]
    public string JacketDesigner { get; init; }

    [Column("jacket_override")]
    public bool JacketOverride { get; init; }

    [Column("audio_override")]
    public bool AudioOverride { get; init; }
}