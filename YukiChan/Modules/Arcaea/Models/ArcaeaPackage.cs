using SQLite;

namespace YukiChan.Modules.Arcaea.Models;

#pragma warning disable CS8618

[Table("packages")]
public class ArcaeaSongDbPackage
{
    [PrimaryKey] [Column("id")] public string Set { get; set; }

    [Column("name")] public string Name { get; set; }
}