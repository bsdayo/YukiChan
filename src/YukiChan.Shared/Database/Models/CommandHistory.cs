using Chloe.Annotations;
using YukiChan.Shared.Common;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace YukiChan.Shared.Database.Models;

[Table("command_history")]
public class CommandHistory
{
    [Column("id", IsPrimaryKey = true)]
    [AutoIncrement]
    public int Id { get; set; }

    [Column("call_time")]
    public required DateTime CallTime { get; set; }

    [Column("respond_time")]
    public required DateTime RespondTime { get; set; }

    [Column("platform")]
    public required string Platform { get; set; }

    [Column("guild_id")]
    public required string GuildId { get; set; }

    [Column("channel_id")]
    public required string ChannelId { get; set; }

    [Column("user_id")]
    public required string UserId { get; set; }

    [Column("user_authority")]
    public required YukiUserAuthority UserAuthority { get; set; }

    [Column("command")]
    public required string Command { get; set; }

    [Column("command_text")]
    public required string CommandText { get; set; }

    [Column("response")]
    public required string? Response { get; set; }

    [Column("is_succeeded")]
    public required bool IsSucceeded { get; set; }
}