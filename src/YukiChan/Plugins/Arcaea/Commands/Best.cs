﻿using ArcaeaUnlimitedAPI.Lib.Models;
using ArcaeaUnlimitedAPI.Lib.Responses;
using ArcaeaUnlimitedAPI.Lib.Utils;
using Flandre.Core.Messaging;
using Flandre.Core.Messaging.Segments;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using Microsoft.Extensions.Logging;
using YukiChan.Plugins.Arcaea.Images;
using YukiChan.Plugins.Arcaea.Models;
using YukiChan.Shared.Database.Models.Arcaea;
using YukiChan.Shared.Utils;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.best <songnameAndDifficulty:text>")]
    [Option("nya", "-n <nya:bool>")]
    [Option("dark", "-d <dark:bool>")]
    [Option("user", "-u <user:string>")]
    [Shortcut("查最高")]
    public async Task<MessageContent> OnBest(MessageContext ctx, ParsedArgs args)
    {
        var userArg = args.GetOption<string>("user");

        var (songname, difficulty) = ArcaeaUtils.ParseMixedSongNameAndDifficulty(
            args.GetArgument<string>("songnameAndDifficulty"));

        try
        {
            var songId = await ArcaeaSongDatabase.FuzzySearchId(songname);
            if (songId is null) return ctx.Reply().Text("没有找到该曲目哦~");

            AuaUserBestContent auaBest;

            if (string.IsNullOrEmpty(userArg)) // 未提供用户名/好友码
            {
                var dbUser = await _database.GetArcaeaUser(ctx.Bot.Platform, ctx.Message.Sender.UserId);
                if (dbUser is null)
                    return ctx.Reply()
                        .Text("请先使用 /a bind 名称或好友码 绑定你的账号哦~\n")
                        .Text("你也可以使用 /a best -u 名称或好友码 直接查询指定用户。");

                _logger.LogInformation(
                    $"正在查询 {ctx.Message.Sender.Name}({ctx.Message.Sender.UserId}) -> {dbUser.ArcaeaName}({dbUser.ArcaeaId}) 的 {songId} 最高成绩...");

                auaBest = int.TryParse(dbUser.ArcaeaId, out var parsed)
                    ? await _service.AuaClient.User.Best(parsed, songId, AuaSongQueryType.SongId, difficulty,
                        AuaReplyWith.All)
                    : await _service.AuaClient.User.Best(dbUser.ArcaeaId, songId, AuaSongQueryType.SongId, difficulty,
                        AuaReplyWith.All);
            }
            else
            {
                _logger.LogInformation($"正在查询 {userArg} 的 {songId} 最高成绩...");
                auaBest = await _service.AuaClient.User.Best(userArg, songId, AuaSongQueryType.SongId, difficulty,
                    AuaReplyWith.All);
            }

            _logger.LogInformation($"正在为 {auaBest.AccountInfo.Name}({auaBest.AccountInfo.Code}) 生成 Best 图片...");

            var pref = await _database.GetArcaeaUserPreferences(ctx.Bot.Platform, ctx.Message.Sender.UserId)
                       ?? new ArcaeaUserPreferences();
            pref.Dark = pref.Dark || args.GetOption<bool>("dark");
            pref.Nya = pref.Nya || args.GetOption<bool>("nya");

            var best = ArcaeaRecord.FromAua(auaBest.Record, auaBest.SongInfo![0]);
            var user = ArcaeaUser.FromAua(auaBest.AccountInfo);
            var image = await ArcaeaImageGenerator.Single(user, best, _service.AuaClient, pref, _logger);

            return ctx.Reply()
                .Text($"{user.Name} ({user.Potential})\n")
                .Image(ImageSegment.FromData(image));
        }
        catch (Exception e)
        {
            if (e is not AuaException)
                _logger.LogError(e, "Error occurred in a.best");
            return ctx.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }
}