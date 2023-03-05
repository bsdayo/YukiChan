using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using Microsoft.Extensions.Logging;
using YukiChan.Core;
using YukiChan.Utils;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.preview")]
    [StringShortcut("查预览", AllowArguments = true)]
    [StringShortcut("查谱面", AllowArguments = true)]
    [StringShortcut("查铺面", AllowArguments = true)]
    public async Task<MessageContent> OnPreview(MessageContext ctx, string[] songnameAndDifficulty)
    {
        var (songname, difficulty) = ArcaeaUtils.ParseMixedSongNameAndDifficulty(songnameAndDifficulty);

        var songId = await _service.SongDb.FuzzySearchId(songname);
        if (songId is null) return ctx.Reply("没有找到该曲目呢...");

        try
        {
            byte[] preview;
            var cachePath = $"{YukiDir.ArcaeaCache}/preview/{songId}-{(int)difficulty}.jpg";

            try
            {
                preview = await File.ReadAllBytesAsync(cachePath);
            }
            catch
            {
                preview = await _yukiClient.Assets.GetArcaeaPreviewImage(songId, difficulty);
                await File.WriteAllBytesAsync(cachePath, preview);
            }

            return ctx.Reply().Image(preview);
        }
        catch (Exception e)
        {
            _logger.LogError(e, string.Empty);
            return ctx.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }
}