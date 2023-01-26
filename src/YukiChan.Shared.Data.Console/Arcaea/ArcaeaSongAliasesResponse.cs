namespace YukiChan.Shared.Data.Console.Arcaea;

public sealed class ArcaeaSongAliasesResponse
{
    public required string[] Aliases { get; init; }

    public required string Name { get; set; }

    public required string Artist { get; set; }
}