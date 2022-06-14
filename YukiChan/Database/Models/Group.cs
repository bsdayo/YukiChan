using SQLite;

namespace YukiChan.Database.Models;

[Table("groups")]
public class YukiGroup
{
    [PrimaryKey, AutoIncrement]
    [Column("id")]
    public uint Id { get; set; }
    
    [Column("uin")] public uint Uin { get; set; }
}