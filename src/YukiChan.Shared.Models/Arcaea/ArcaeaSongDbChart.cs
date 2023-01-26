using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618

namespace YukiChan.Shared.Models.Arcaea;

[Table("charts")]
public sealed class ArcaeaSongDbChart : ArcaeaChart
{
    [Column("song_id")]
    [Key]
    public string SongId { get; init; }

    [Column("set")]
    public string Set { get; init; }
}