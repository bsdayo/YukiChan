﻿using ArcaeaUnlimitedAPI.Lib.Models;
using ArcaeaUnlimitedAPI.Lib.Responses;
using ArcaeaUnlimitedAPI.Lib.Utils;
using Flandre.Core.Attributes;
using Flandre.Core.Common;
using Flandre.Core.Messaging;
using Flandre.Core.Messaging.Segments;
using YukiChan.Plugins.Arcaea.Images;
using YukiChan.Plugins.Arcaea.Models;
using YukiChan.Plugins.Arcaea.Models.Database;
using YukiChan.Utils;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.alias <songname: string>")]
    [Shortcut("查别名")]
    public async Task<MessageContent> OnAlias(MessageContext ctx, ParsedArgs args)
    {
        var songId = await ArcaeaSongDatabase.FuzzySearchId(args.GetArgument<string>("songname"));
        if (songId is null) return ctx.Reply("没有找到该曲目哦~");

        var chart = await ArcaeaSongDatabase.GetChartsById(songId);
        var aliases = (await ArcaeaSongDatabase.GetAliasesById(songId))
            .Select(alias => alias.Alias);
            
        return ctx.Reply()
            .Text($"{chart[0].NameEn} - {chart[0].Artist}\n")
            .Text("可用的别名有：\n")
            .Text(string.Join('\n', aliases));
    }
}