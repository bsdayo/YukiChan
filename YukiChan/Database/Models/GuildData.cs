﻿using Chloe.Annotations;

namespace YukiChan.Database.Models;

[Table("guilds")]
public class GuildData
{
    [Column("id", IsPrimaryKey = true)]
    [AutoIncrement]
    public int Id { get; set; }

    [Column("platform")] public required string Platform { get; set; }

    [Column("guild_id")] public required string GuildId { get; set; }

    [Column("assignee")] public required string Assignee { get; set; }
}