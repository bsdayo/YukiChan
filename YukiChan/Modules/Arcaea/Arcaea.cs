﻿using ArcaeaUnlimitedAPI.Lib;
using YukiChan.Core;

namespace YukiChan.Modules.Arcaea;

[Module("Arcaea",
    Command = "a",
    Description = "Arcaea 相关功能",
    Version = "1.0.0")]
public partial class ArcaeaModule : ModuleBase
{
    private static readonly AuaClient AuaClient = new AuaClient
    {
        ApiUrl = Global.YukiConfig.Arcaea.AuaApiUrl,
        UserAgent = Global.YukiConfig.Arcaea.UserAgent,
        Timeout = Global.YukiConfig.Arcaea.Timeout
    }.Initialize();
}