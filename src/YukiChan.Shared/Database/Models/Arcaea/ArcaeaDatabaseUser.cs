using Chloe.Annotations;

#pragma warning disable CS8618

namespace YukiChan.Shared.Database.Models.Arcaea;

[Table("arcaea_users")]
public class ArcaeaDatabaseUser
{
    [Column("id", IsPrimaryKey = true)]
    [NotNull]
    public int Id { get; set; }

    [Column("platform")] [NotNull] public string Platform { get; set; }

    [Column("user_id")] [NotNull] public string UserId { get; set; }

    [Column("arcaea_id")] [NotNull] public string ArcaeaId { get; set; }

    [Column("arcaea_name")] public string ArcaeaName { get; set; } = "";
}