using System.ComponentModel.DataAnnotations.Schema;

namespace YukiChan.Modules.Arcaea.Models;

#pragma warning disable CS8618

public class ArcaeaSongDbPackage
{
    [Column("id")] public string Set { get; set; }

    [Column("name")] public string Name { get; set; }
}