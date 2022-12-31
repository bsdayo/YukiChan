using ArcaeaUnlimitedAPI.Lib.Models;
using ArcaeaUnlimitedAPI.Lib.Utils;
using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using Microsoft.Extensions.Logging;
using YukiChan.Shared;
using YukiChan.Shared.Arcaea;
using YukiChan.Shared.Utils;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.preview <songnameAndDifficulty:text>")]
    [Shortcut("查预览")]
    public async Task<MessageContent> OnPreview(MessageContext ctx, ParsedArgs args)
    {
        var (songname, difficulty) = ArcaeaUtils.ParseMixedSongNameAndDifficulty(
            args.GetArgument<string>("songnameAndDifficulty"));

        var songId = await ArcaeaSongDatabase.Default.FuzzySearchId(songname);
        if (songId is null) return ctx.Reply("没有找到该曲目呢...");

        try
        {
            byte[] preview;
            var cachePath = $"{YukiDir.ArcaeaCache}/preview/{songId}-{difficulty.ToString().ToLower()}.jpg";

            try
            {
                preview = await File.ReadAllBytesAsync(cachePath);
            }
            catch
            {
                preview = await _service.AuaClient.Assets.Preview(songId, AuaSongQueryType.SongId, difficulty);
                await File.WriteAllBytesAsync(cachePath, preview);
                _logger.SaveCache(cachePath);
            }

            return ctx.Reply().Image(preview);
        }
        catch (AuaException e)
        {
            var errMsg = AuaErrorStatus.GetMessage(e.Status, e.Message);
            return ctx.Reply(errMsg);
        }
        catch (YukiException e)
        {
            _logger.LogError(e, string.Empty);
            return ctx.Reply(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, string.Empty);
            return ctx.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }
}