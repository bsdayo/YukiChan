using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YukiChan.Shared.Models.Arcaea;

#pragma warning disable CS8618

[Table("packages")]
public sealed class ArcaeaSongDbPackage
{
    [Column("id")]
    [Key]
    public string Set { get; set; }

    [Column("name")]
    public string Name { get; set; }
}