using SQLite;

namespace YukiChan.Modules.Arcaea.Models;

#pragma warning disable CS8618

[Table("alias")]
public class ArcaeaSongDbAlias
{
    [Column("sid")] public string SongId { get; set; }

    [Column("alias")] public string Alias { get; set; }
}