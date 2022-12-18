using ArcaeaUnlimitedAPI.Lib.Models;
using YukiChan.Shared.Arcaea.Models;
using YukiChan.Shared.Utils;

namespace YukiChan.Shared.Arcaea.Factories;

public static class ArcaeaUserFactory
{
    public static ArcaeaUser FromAua(AuaAccountInfo info)
    {
        return new ArcaeaUser
        {
            Name = info.Name,
            Id = info.Code,
            Potential = info.Rating >= 0
                ? ((double)info.Rating / 100).ToString("0.00")
                : "?",
            JoinDate = info.JoinDate.FormatTimestamp(true)
        };
    }

    public static ArcaeaUser FromAla(AlaUser info, string usercode)
    {
        return new ArcaeaUser
        {
            Name = info.DisplayName,
            Id = usercode,
            Potential = info.Potential >= 0
                ? ((double)info.Potential / 100).ToString("0.00")
                : "?",
            JoinDate = "?"
        };
    }

    public static ArcaeaUser FromAla(AlaUser info, int usercode)
    {
        return FromAla(info, usercode.ToString().PadLeft(9, '0'));
    }
}