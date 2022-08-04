using SQLite;

#pragma warning disable CS8618

namespace YukiChan.Modules.Arcaea.Models;

[Table("arcaea_users")]
public class ArcaeaDatabaseUser
{
    [PrimaryKey] [Column("uin")] public uint Uin { get; set; }

    [Column("id")] public string Id { get; set; }

    [Column("name")] public string Name { get; set; }
}