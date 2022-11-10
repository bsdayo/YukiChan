using ArcaeaUnlimitedAPI.Lib.Models;
using ArcaeaUnlimitedAPI.Lib.Utils;
using Flandre.Core.Attributes;
using Flandre.Core.Common;
using Flandre.Core.Messaging;
using YukiChan.Utils;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.preview <songname: string> [difficulty: string = ftr]")]
    [Shortcut("查预览")]
    public async Task<MessageContent> OnPreview(MessageContext ctx, ParsedArgs args)
    {
        var difficulty = ArcaeaUtils.GetRatingClass(args.GetArgument<string>("difficulty"));
        if (difficulty is null)
            return ctx.Reply("输入了错误的难度！");

        var songId = await ArcaeaSongDatabase.FuzzySearchId(args.GetArgument<string>("songname"));
        if (songId is null) return ctx.Reply("没有找到该曲目呢...");

        try
        {
            byte[] preview;
            var cachePath = $"{YukiDir.ArcaeaCache}/preview/{songId}-{difficulty.ToString()!.ToLower()}.jpg";

            try
            {
                preview = await File.ReadAllBytesAsync(cachePath);
            }
            catch
            {
                preview = await _auaClient.Assets.Preview(songId, AuaSongQueryType.SongId, difficulty.Value);
                await File.WriteAllBytesAsync(cachePath, preview);
                Logger.SaveCache(cachePath);
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
            Logger.Error(e);
            return ctx.Reply(e.Message);
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return ctx.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }
}