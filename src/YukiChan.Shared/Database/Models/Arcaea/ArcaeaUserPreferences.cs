using Chloe.Annotations;

namespace YukiChan.Shared.Database.Models.Arcaea;

[Table("arcaea_preferences")]
public class ArcaeaUserPreferences
{
    [Column("id", IsPrimaryKey = true)]
    [NotNull]
    public int Id { get; set; }

    [Column("platform")] [NotNull] public string Platform { get; set; } = null!;

    [Column("user_id")] [NotNull] public string UserId { get; set; } = null!;

    [Column("dark")] public bool Dark { get; set; }

    [Column("nya")] public bool Nya { get; set; }
}