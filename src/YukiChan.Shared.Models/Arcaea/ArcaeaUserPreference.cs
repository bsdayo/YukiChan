using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YukiChan.Shared.Models.Arcaea;

[Table("arcaea_preferences")]
public class ArcaeaUserPreferences
{
    [Column("id")]
    [Required]
    [Key]
    public int Id { get; set; }

    [Column("platform")]
    [Required]
    public string Platform { get; set; } = null!;

    [Column("user_id")]
    [Required]
    public string UserId { get; set; } = null!;

    [Column("dark")]
    public bool Dark { get; set; }

    [Column("nya")]
    public bool Nya { get; set; }

    [Column("single_dynamic_bg")]
    public bool SingleDynamicBackground { get; set; } = true;

    [Column("b30_show_grade")]
    public bool Best30ShowGrade { get; set; } = true;
}