namespace YukiChan.Shared.Data.Console;

public sealed class PrecheckRequest
{
    public required string Platform { get; init; }

    public string? GuildId { get; init; }
    
    public string? ChannelId { get; init; }

    public required string UserId { get; init; }

    public required string SelfId { get; init; }
    
    public required YukiEnvironment Environment { get; init; }

    public required string Command { get; init; }

    public required string CommandText { get; init; }
}

public enum YukiEnvironment
{
    Channel = 0,
    Private = 1
}