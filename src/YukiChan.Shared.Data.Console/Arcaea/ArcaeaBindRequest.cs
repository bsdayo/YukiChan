using System.ComponentModel.DataAnnotations;

namespace YukiChan.Shared.Data.Console.Arcaea;

public sealed class ArcaeaBindRequest
{
    [Required]
    public required string BindTarget { get; init; }
}