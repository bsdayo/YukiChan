namespace YukiChan.Shared.Data.Console.Guilds;

public sealed class GuildUpdateAssigneeRequest
{
    public required string NewAssigneeId { get; init; }
}