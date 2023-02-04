namespace YukiChan.Shared.Data.Console.Arcaea;

public sealed class ArcaeaSubmitAliasRequest
{
    public required string Platform { get; init; }

    public required string UserId { get; init; }

    public required string SongId { get; init; }

    public required string Alias { get; init; }
}