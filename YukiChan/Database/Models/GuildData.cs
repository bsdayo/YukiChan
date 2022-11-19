using Chloe.Annotations;

namespace YukiChan.Database.Models;

public class GuildData
{
    [Column("platform")] public required string Platform { get; set; }

    [Column("guild_id")] public required string GuildId { get; set; }

    [Column("assignee")] public required string Assignee { get; set; }
}