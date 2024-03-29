﻿using Flandre.Core.Messaging;
using Flandre.Core.Messaging.Segments;
using Flandre.Framework.Attributes;
using Microsoft.Extensions.Logging;
using YukiChan.Shared.Data;
using YukiChan.Shared.Data.Console.Arcaea;
using YukiChan.Shared.Models.Arcaea;
using YukiChan.Shared.Utils;
using YukiChan.Utils;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.recent", "a")]
    [StringShortcut("查最近", AllowArguments = true)]
    public async Task<MessageContent> OnRecent(MessageContext ctx,
        [Option(ShortName = 'n')] bool nya,
        [Option(ShortName = 'd')] bool dark,
        [Option(ShortName = 'u')] string userArg) // TODO: fix shortname not effected
    {
        try
        {
            YukiResponse<ArcaeaRecentResponse> recentResp;

            if (string.IsNullOrEmpty(userArg)) // 未提供用户名/好友码
            {
                var userResp = await _yukiClient.Arcaea.GetUser(ctx.Platform, ctx.UserId);
                if (userResp.Code == YukiErrorCode.Arcaea_NotBound)
                    return ctx.Reply()
                        .Text("请先使用 /a bind 名称或好友码 绑定你的账号哦~\n")
                        .Text("你也可以使用 /a recent -u 名称或好友码 直接查询指定用户。");
                if (!userResp.Ok) return ctx.ReplyServerError(userResp);

                _logger.LogInformation(
                    "正在查询 {UserName}({UserId}) -> {ArcaeaName}({ArcaeaId}) 的最近成绩...",
                    ctx.Message.Sender.Name, ctx.Message.Sender.UserId,
                    userResp.Data.ArcaeaName, userResp.Data.ArcaeaId);

                recentResp = await _yukiClient.Arcaea.GetRecent(userResp.Data.ArcaeaId);
            }
            else
            {
                _logger.LogInformation("正在查询 {ArcaeaName} 的最近一条成绩...", userArg);
                recentResp = await _yukiClient.Arcaea.GetRecent(userArg);
            }

            if (recentResp.Code == YukiErrorCode.Arcaea_NotPlayedRecently)
                return ctx.Reply("该用户最近还没有游玩过哦~");
            if (!recentResp.Ok) return ctx.ReplyServerError(recentResp);

            var user = recentResp.Data.User;
            var record = recentResp.Data.RecentRecord;

            var prefResp = await _yukiClient.Arcaea.GetPreferences(ctx.Platform, ctx.UserId);
            var pref = prefResp.Ok ? prefResp.Data.Preferences : new ArcaeaUserPreferences();
            pref.Nya = pref.Nya || nya;
            pref.Dark = pref.Dark || dark;

            _logger.LogInformation("正在为 {ArcaeaName} ({ArcaeaId}) 生成最近成绩图查...", user.Name, user.Code);
            var image = await _service.ImageGenerator.SingleV1(user, record, _yukiClient, pref, _logger);

            return ctx.Reply()
                .Text(
                    $"{recentResp.Data.User.Name} ({ArcaeaSharedUtils.ToDisplayPotential(recentResp.Data.User.Potential)})\n")
                .Image(ImageSegment.FromData(image));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred in a.recent");
            return ctx.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }
}