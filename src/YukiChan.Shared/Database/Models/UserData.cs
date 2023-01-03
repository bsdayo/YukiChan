using Chloe.Annotations;
using YukiChan.Shared.Common;

namespace YukiChan.Shared.Database.Models;

[Table("users")]
public class UserData
{
    [Column("id", IsPrimaryKey = true)]
    [AutoIncrement]
    public int Id { get; set; }

    [Column("platform")]
    public required string Platform { get; set; }

    [Column("user_id")]
    public required string UserId { get; set; }

    [Column("user_authority")]
    public YukiUserAuthority UserAuthority { get; set; } = YukiUserAuthority.User;
}