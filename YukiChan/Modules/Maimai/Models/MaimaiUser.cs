using SQLite;

#pragma warning disable CS8618

namespace YukiChan.Modules.Maimai.Models;

[Table("maimai_users")]
public class MaimaiUser
{
    [PrimaryKey] [Column("uin")] public uint Uin { get; set; }

    [Column("name")] public string Name { get; set; }
}