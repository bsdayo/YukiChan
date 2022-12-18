using Chloe.Annotations;

namespace YukiChan.Shared.Arcaea.Models;

#pragma warning disable CS8618

[Table("packages")]
public sealed class ArcaeaSongDbPackage
{
    [Column("id", IsPrimaryKey = true)] public string Set { get; set; }

    [Column("name")] public string Name { get; set; }
}