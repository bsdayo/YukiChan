using Chloe.Annotations;

#pragma warning disable CS8618

namespace YukiChan.Plugins.Arcaea.Models;

[Table("arcaea_users")]
public class ArcaeaDatabaseUser
{
    [Column("uin", IsPrimaryKey = true)] public uint Uin { get; set; }

    [Column("id")] public string Id { get; set; }

    [Column("name")] public string Name { get; set; }
}