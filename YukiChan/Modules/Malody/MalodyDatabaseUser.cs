using SQLite;

#pragma warning disable CS8618

namespace YukiChan.Modules.Malody;

[Table("malody_users")]
public class MalodyDatabaseUser
{
    [PrimaryKey] [Column("uin")] public uint Uin { get; set; }

    [Column("id")] public int Id { get; set; }

    [Column("name")] public string Name { get; set; }
}