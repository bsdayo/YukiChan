using ArcaeaUnlimitedAPI.Lib.Models;
using YukiChan.Utils;

#pragma warning disable CS8618

namespace YukiChan.Modules.Arcaea.Models;

public class ArcaeaUser
{
    public string Name { get; private init; }

    public string Id { get; private init; }

    public string Potential { get; private init; }

    public string JoinDate { get; private init; }

    public static ArcaeaUser FromAua(AuaAccountInfo info)
    {
        return new ArcaeaUser
        {
            Name = info.Name,
            Id = info.Code,
            Potential = info.Rating >= 0
                ? ((double)info.Rating / 100).ToString("0.00")
                : "?",
            JoinDate = CommonUtils.FormatTimestamp(info.JoinDate, true)
        };
    }
}