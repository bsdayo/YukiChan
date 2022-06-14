using SQLite;

namespace YukiChan.Database.Models;

[Table("users")]
public class YukiUser
{
    [PrimaryKey, AutoIncrement]
    [Column("id")]
    public uint Id { get; set; }

    [Column("uin")] public uint Uin { get; set; }

    [Column("authority")] public YukiUserAuthority Authority { get; set; } = YukiUserAuthority.User;
}

public enum YukiUserAuthority
{
    Banned = 0,
    User = 1,
    Admin = 2,
    Owner = 3
}