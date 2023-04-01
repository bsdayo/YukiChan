using System.Text.Json;
using System.Text.Json.Serialization;
using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YukiChan.Core;
using YukiChan.Utils;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

file class ArcaeaAf2023Json
{
    [JsonPropertyName("en")]
    public string[] En { get; set; } = Array.Empty<string>();

    [JsonPropertyName("ko")]
    public string[] Ko { get; set; } = Array.Empty<string>();

    [JsonPropertyName("ja")]
    public string[] Ja { get; set; } = Array.Empty<string>();

    [JsonPropertyName("zh-Hans")]
    public string[] ZhHans { get; set; } = Array.Empty<string>();

    [JsonPropertyName("zh-Hant")]
    public string[] ZhHant { get; set; } = Array.Empty<string>();
}

public partial class ArcaeaPlugin
{
    [Command("a.suggest")]
    [RegexShortcut( /* lang=regex */ @"^[能再]?推荐一首歌(给我[吧吗]?[！\!？\?]?)?$")]
    [RegexShortcut( /* lang=regex */
        @"^(啊?[，,]?(不是很想)?)?这?(\.\.\.)?(\.\.\.)?…?…?(谢谢[！\!]?)?(能不能)?(要不)?(还是)?换一首[吧哇]?[！\!？\?]?$")]
    [RegexShortcut( /* lang=regex */ @"^(你觉得我)?打哪一首歌好呢?[？\?]?$")]
    public async Task<MessageContent> OnSuggest(CommandContext ctx)
    {
        try
        {
            var jsonPath = $"{YukiDir.ArcaeaAssets}/af2023.json";
            var data = File.Exists(jsonPath)
                ? JsonSerializer.Deserialize<ArcaeaAf2023Json>(await File.ReadAllTextAsync(jsonPath))!
                : new ArcaeaAf2023Json();

            if (!data.ZhHans.Any())
                return ctx.Reply("当前没有可用的数据！");

            var allSongs = (await _service.SongDb.Charts.ToListAsync())
                .DistinctBy(chart => chart.SongId)
                .ToArray();

            var random = allSongs[new Random().Next(allSongs.Length)];

            var text = data.ZhHans[new Random().Next(data.ZhHans.Length)]
                .Replace("歌曲名称2", "歌曲名称")
                .Replace("“歌曲名称”", $"“{random.NameEn}”")
                .Replace("“作曲家”", $"“{random.Artist}”");

            return ctx.Reply(text);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
            return null!;
        }
    }
}