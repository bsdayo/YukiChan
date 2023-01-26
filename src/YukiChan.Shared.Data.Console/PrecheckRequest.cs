namespace YukiChan.Shared.Data.Console;

public sealed class PrecheckRequest
{
    public required string Platform { get; init; }

    public string? GuildId { get; init; }

    public required string UserId { get; init; }

    public required string SelfId { get; init; }

    public required string Command { get; init; }

    public required string CommandText { get; init; }
}