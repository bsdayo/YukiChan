using Chloe.Annotations;

namespace YukiChan.Plugins.Arcaea.Models;

#pragma warning disable CS8618

[Table("alias")]
public class ArcaeaSongDbAlias
{
    [Column("sid")] public string SongId { get; set; }

    [Column("alias", IsPrimaryKey = true)] public string Alias { get; set; }
}