﻿using ArcaeaUnlimitedAPI.Lib.Models;
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
            Code = info.Code,
            Potential = info.Rating >= 0
                ? ((double)info.Rating / 100).ToString("0.00")
                : "?",
            JoinDate = DateTimeUtils.FormatUtc8Text(info.JoinDate, true)
        };
    }

    public static ArcaeaUser FromAla(AlaUser info, string usercode)
    {
        return new ArcaeaUser
        {
            Name = info.DisplayName,
            Code = usercode,
            Potential = info.Potential >= 0
                ? ((double)info.Potential / 100).ToString("0.00")
                : "?",
            JoinDate = "?"
        };
    }

    public static ArcaeaUser GenerateFake()
    {
        return new ArcaeaUser
        {
            Name = "yyw",
            Code = "007355608",
            Potential = "99.99",
            JoinDate = DateTime.UnixEpoch.ToString("yyyy.MM.dd HH:mm:ss")
        };
    }
}