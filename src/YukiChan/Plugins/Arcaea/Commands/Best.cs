using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using Microsoft.Extensions.Logging;
using YukiChan.Shared.Data;
using YukiChan.Shared.Models.Arcaea;
using YukiChan.Shared.Utils;
using YukiChan.Utils;

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
            var userResp = await _yukiClient.Arcaea.GetUser(ctx.Platform, ctx.UserId);
            if (userResp.Code == YukiErrorCode.Arcaea_NotBound)
                return ctx.Reply("请先使用 /a bind 名称或好友码 绑定你的账号哦~");
            if (!userResp.Ok) return ctx.ReplyServerError(userResp);

            var bestResp = await _yukiClient.Arcaea.GetBest(
                userResp.Data.ArcaeaId, songname, difficulty);
            if (!bestResp.Ok) return ctx.ReplyServerError(bestResp);
            var best = bestResp.Data;

            var prefResp = await _yukiClient.Arcaea.GetPreferences(ctx.Platform, ctx.UserId);
            var pref = prefResp.Ok ? prefResp.Data.Preferences : new ArcaeaUserPreferences();

            var image = await _service.ImageGenerator.SingleV1(best.User, best.BestRecord,
                _yukiClient, pref, _logger);

            return ctx
                .Reply($"{best.User.Name} ({ArcaeaSharedUtils.ToDisplayPotential(best.User.Potential)})\n")
                .Image(image);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred in a.best");
            return ctx.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }
}