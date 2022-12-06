using Chloe.Annotations;

#pragma warning disable CS8618

namespace YukiChan.Shared.Database.Models.Arcaea;

[Table("arcaea_guess")]
public class ArcaeaGuessUser
{
    [Column("id", IsPrimaryKey = true)]
    [AutoIncrement]
    public int Id { get; set; } = 1;

    [Column("year")] public int Year { get; set; }

    [Column("month")] public int Month { get; set; }

    [Column("day")] public int Day { get; set; }

    [Column("group_uin")] public uint GroupUin { get; set; }

    [Column("group_name")] public string GroupName { get; set; }

    [Column("user_uin")] public uint UserUin { get; set; }

    [Column("user_name")] public string UserName { get; set; }

    [Column("easy_correct_count")] public int EasyCorrectCount { get; set; } = 0;
    [Column("easy_wrong_count")] public int EasyWrongCount { get; set; } = 0;

    [Column("normal_correct_count")] public int NormalCorrectCount { get; set; } = 0;
    [Column("normal_wrong_count")] public int NormalWrongCount { get; set; } = 0;

    [Column("hard_correct_count")] public int HardCorrectCount { get; set; } = 0;
    [Column("hard_wrong_count")] public int HardWrongCount { get; set; } = 0;

    [Column("flash_correct_count")] public int FlashCorrectCount { get; set; } = 0;
    [Column("flash_wrong_count")] public int FlashWrongCount { get; set; } = 0;

    [Column("grayscale_correct_count")] public int GrayScaleCorrectCount { get; set; } = 0;
    [Column("grayscale_wrong_count")] public int GrayScaleWrongCount { get; set; } = 0;

    [Column("invert_correct_count")] public int InvertCorrectCount { get; set; } = 0;
    [Column("invert_wrong_count")] public int InvertWrongCount { get; set; } = 0;

    [NotMapped]
    public double EasyCorrectRate =>
        (double)EasyCorrectCount / (EasyCorrectCount + EasyWrongCount);

    [NotMapped]
    public double NormalCorrectRate =>
        (double)NormalCorrectCount / (NormalCorrectCount + NormalWrongCount);

    [NotMapped]
    public double HardCorrectRate =>
        (double)HardCorrectCount / (HardCorrectCount + HardWrongCount);

    [NotMapped]
    public double FlashCorrectRate =>
        (double)FlashCorrectCount / (FlashCorrectCount + FlashWrongCount);

    [NotMapped]
    public double GrayScaleCorrectRate =>
        (double)GrayScaleCorrectCount / (GrayScaleCorrectCount + GrayScaleWrongCount);

    [NotMapped]
    public double InvertCorrectRate =>
        (double)InvertCorrectCount / (InvertCorrectCount + InvertWrongCount);
}