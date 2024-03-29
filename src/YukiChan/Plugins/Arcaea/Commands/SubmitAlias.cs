﻿using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using YukiChan.Shared.Data;
using YukiChan.Shared.Data.Console.Arcaea;
using YukiChan.Shared.Models.Arcaea;
using YukiChan.Shared.Utils;
using YukiChan.Utils;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.submit-alias")]
    public async Task<MessageContent> OnSubmitAlias(MessageContext ctx, params string[] songnameAndAlias)
    {
        if (songnameAndAlias.Length < 2)
            return ctx.Reply("指令格式错误。");

        var query = string.Join(' ', songnameAndAlias[..^1]);
        var alias = songnameAndAlias[^1];

        var songIdResp = await _yukiClient.Arcaea.QuerySongId(query);
        if (songIdResp.Code == YukiErrorCode.Arcaea_SongNotFound)
            return ctx.Reply("没有找到该曲目哦~");
        if (!songIdResp.Ok)
            return ctx.ReplyServerError(songIdResp);

        var submitResp = await _yukiClient.Arcaea.SubmitAlias(new ArcaeaSubmitAliasRequest
        {
            Platform = ctx.Platform,
            UserId = ctx.UserId,
            SongId = songIdResp.Data.SongId,
            Alias = alias
        });

        switch (submitResp.Code)
        {
            case YukiErrorCode.Arcaea_AliasAlreadyExists:
                return ctx.Reply("数据库中已经有这个别名了哦~\n");

            case YukiErrorCode.Arcaea_AliasSubmissionAlreadyExists:
                return ctx.Reply("这个别名已经被申请过了哦~\n")
                    .Text($"你可以使用 /a submit-alias-status {submitResp.Data.SubmissionId} 查看处理状态。");
        }

        if (!submitResp.Ok)
            return ctx.ReplyServerError(submitResp);

        return ctx.Reply("申请成功！\n")
            .Text($"你可以使用 /a submit-alias-status {submitResp.Data.SubmissionId} 查看处理状态。");
    }

    [Command("a.submit-alias-status")]
    public async Task<MessageContent> OnSubmitAliasStatus(MessageContext ctx, int submissionId)
    {
        var resp = await _yukiClient.Arcaea.GetAliasSubmission(submissionId);
        if (resp.Code == YukiErrorCode.NotFound)
            return ctx.Reply("没有找到这个申请哦！");
        if (!resp.Ok) return ctx.ReplyServerError(resp);

        var songResp = await _yukiClient.Arcaea.QuerySong(resp.Data.SongId);
        if (!songResp.Ok) return ctx.ReplyServerError(songResp);

        var statusText = resp.Data.Status switch
        {
            ArcaeaAliasSubmissionStatus.Rejected => "已拒绝",
            ArcaeaAliasSubmissionStatus.Accepted => "已通过",
            _ => "处理中"
        };

        return ctx.Reply($"别名申请 #{resp.Data.Id}\n")
            .Text($"曲目：{songResp.Data.Difficulties[0].NameEn}\n")
            .Text($"别名：{resp.Data.Alias}\n")
            .Text($"申请时间：{resp.Data.SubmitTime.ToDisplayText(true)}\n")
            .Text($"状态：{statusText}");
    }
}