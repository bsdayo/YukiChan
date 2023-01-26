using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YukiChan.Shared.Models.Arcaea;

#pragma warning disable CS8618

[Table("alias")]
public sealed class ArcaeaSongDbAlias
{
    [Column("sid")]
    public string SongId { get; set; }

    [Column("alias")]
    [Key]
    public string Alias { get; set; }
}