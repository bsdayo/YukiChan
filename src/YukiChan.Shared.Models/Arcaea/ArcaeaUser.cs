#pragma warning disable CS8618

namespace YukiChan.Shared.Models.Arcaea;

public sealed class ArcaeaUser
{
    public string Name { get; set; }

    public string Code { get; set; }

    public double Potential { get; set; }

    public DateTime JoinTime { get; set; }

    public int PartnerId { get; set; }

    public bool IsPartnerAwakened { get; set; }
}