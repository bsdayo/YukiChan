﻿using Flandre.Core.Messaging;
using Flandre.Core.Messaging.Segments;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using Microsoft.Extensions.Logging;
using YukiChan.Shared.Data;
using YukiChan.Shared.Models.Arcaea;
using YukiChan.Utils;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.b30 [user: string]")]
    [Option("nya", "-n <:bool>")]
    [Option("dark", "-d <:bool>")]
    [Option("official", "-o <:bool>")]
    [Shortcut("查b30")]
    [Shortcut("查B30")]
    public async Task<MessageContent> OnBest30(MessageContext ctx, ParsedArgs args)
    {
        var userArg = args.GetArgument<string>("user");
        var official = args.GetOption<bool>("official");

        try
        {
            string arcName, arcId;

            if (string.IsNullOrEmpty(userArg)) // 未提供用户名/好友码
            {
                var userResp = await _yukiClient.Arcaea.GetUser(ctx.Platform, ctx.UserId);
                if (userResp.Code == YukiErrorCode.Arcaea_NotBound)
                    return ctx.Reply()
                        .Text("请先使用 /a bind 名称或好友码 绑定你的账号哦~\n")
                        .Text("你也可以使用 /a b30 名称或好友码 直接查询指定用户。");
                if (!userResp.Ok) return ctx.ReplyServerError(userResp);
                arcName = userResp.Data.ArcaeaName;
                arcId = userResp.Data.ArcaeaId;
            }
            else
            {
                if (official && (userArg.Length > 9 || !int.TryParse(userArg, out _)))
                    return ctx.Reply("官方 API 仅支持好友码查询。");
                arcName = arcId = userArg;
            }

            await ctx.Bot.SendMessage(ctx.Message, official
                ? $"正在使用官方 API 查询 {arcName} 的 Best30 成绩，请耐心等候..."
                : $"正在查询 {arcName} 的 Best30 成绩，请耐心等候...");
            _logger.LogInformation("正在查询 {ArcaeaName} ({ArcaeaId}) 的 Best30 成绩...", arcName, arcId);

            var best30Resp = await _yukiClient.Arcaea.GetBest30(arcId, official);
            if (!best30Resp.Ok) return ctx.ReplyServerError(best30Resp);

            var prefResp = await _yukiClient.Arcaea.GetPreferences(
                ctx.Bot.Platform, ctx.Message.Sender.UserId);
            var pref = prefResp.Ok ? prefResp.Data.Preferences : new ArcaeaUserPreferences();
            pref.Dark = pref.Dark || args.GetOption<bool>("dark");
            pref.Nya = pref.Nya || args.GetOption<bool>("nya");

            _logger.LogInformation("正在为 {ArcaeaName} ({ArcaeaId}) 生成 Best30 图查...", arcName, arcId);
            var image = await _service.ImageGenerator.Best30(best30Resp.Data.Best30, pref, _yukiClient, _logger);

            return ctx.Reply().Image(ImageSegment.FromData(image));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred in a.b30");
            return ctx.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }
}