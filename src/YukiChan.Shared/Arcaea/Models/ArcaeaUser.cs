#pragma warning disable CS8618

namespace YukiChan.Shared.Arcaea.Models;

public sealed class ArcaeaUser
{
    public string Name { get; init; }

    public string Id { get; init; }

    public string Potential { get; init; }

    public string JoinDate { get; init; }
}