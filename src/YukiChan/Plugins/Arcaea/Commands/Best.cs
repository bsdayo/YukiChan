using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using Microsoft.Extensions.Logging;
using YukiChan.Shared.Data;
using YukiChan.Shared.Models.Arcaea;
using YukiChan.Shared.Utils;
using YukiChan.Utils;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.best")]
    [StringShortcut("查最高", AllowArguments = true)]
    public async Task<MessageContent> OnBest(MessageContext ctx,
        string[] songnameAndDifficulty,
        [Option(ShortName = 'n')] bool nya,
        [Option(ShortName = 'd')] bool dark,
        [Option(ShortName = 'u')] string user)
    {
        var (songname, difficulty) = ArcaeaUtils.ParseMixedSongNameAndDifficulty(songnameAndDifficulty);

        try
        {
            string target;

            if (string.IsNullOrEmpty(user))
            {
                var userResp = await _yukiClient.Arcaea.GetUser(ctx.Platform, ctx.UserId);
                if (userResp.Code == YukiErrorCode.Arcaea_NotBound)
                    return ctx.Reply("请先使用 /a bind 名称或好友码 绑定你的账号哦~");
                if (!userResp.Ok) return ctx.ReplyServerError(userResp);
                target = userResp.Data.ArcaeaId;
            }
            else
            {
                target = user;
            }

            _logger.LogInformation("正在查询 {Target} 的 {SongName} [{Difficulty}] 最高成绩...",
                target, songname, difficulty.ToShortDisplayDifficulty());

            var bestResp = await _yukiClient.Arcaea.GetBest(
                target, songname, difficulty);
            if (!bestResp.Ok) return ctx.ReplyServerError(bestResp);
            var best = bestResp.Data;

            var prefResp = await _yukiClient.Arcaea.GetPreferences(ctx.Platform, ctx.UserId);
            var pref = prefResp.Ok ? prefResp.Data.Preferences : new ArcaeaUserPreferences();

            pref.Nya = pref.Nya || nya;
            pref.Dark = pref.Dark || dark;

            _logger.LogInformation("正在为 {Target} 生成 {SongName} [{Difficulty}] 最高成绩的图查...",
                target, songname, difficulty.ToShortDisplayDifficulty());

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