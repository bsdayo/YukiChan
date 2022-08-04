﻿using SQLite;

#pragma warning disable CS8618

namespace YukiChan.Modules.Arcaea.Models;

[Table("arcaea_guess")]
public class ArcaeaGuessUser
{
    [PrimaryKey]
    [AutoIncrement]
    [Column("id")]
    public int Id { get; set; }

    [Column("year")] public int Year { get; set; }

    [Column("month")] public int Month { get; set; }

    [Column("day")] public int Day { get; set; }

    [Column("group_uin")] public uint GroupUin { get; set; }

    [Column("user_uin")] public uint UserUin { get; set; }

    [Column("user_name")] public string UserName { get; set; }

    [Column("easy_correct_count")] public int EasyCorrectCount { get; set; } = 0;

    [Column("easy_wrong_count")] public int EasyWrongCount { get; set; } = 0;

    [Column("normal_correct_count")] public int NormalCorrectCount { get; set; } = 0;

    [Column("normal_wrong_count")] public int NormalWrongCount { get; set; } = 0;

    [Column("hard_correct_count")] public int HardCorrectCount { get; set; } = 0;

    [Column("hard_wrong_count")] public int HardWrongCount { get; set; } = 0;

    public double EasyCorrectRate => (double)EasyCorrectCount / (EasyCorrectCount + EasyWrongCount);
    public double NormalCorrectRate => (double)NormalCorrectCount / (NormalCorrectCount + NormalWrongCount);
    public double HardCorrectRate => (double)HardCorrectCount / (HardCorrectCount + HardWrongCount);
}