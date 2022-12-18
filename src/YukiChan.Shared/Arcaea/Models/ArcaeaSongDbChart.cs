using Chloe.Annotations;

#pragma warning disable CS8618

namespace YukiChan.Shared.Arcaea.Models;

[Table("charts")]
public sealed class ArcaeaSongDbChart : ArcaeaChart
{
    [Column("song_id", IsPrimaryKey = true)]
    public string SongId { get; init; }

    [Column("set")] public string Set { get; init; }
}