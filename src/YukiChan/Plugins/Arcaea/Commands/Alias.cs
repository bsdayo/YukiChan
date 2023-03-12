using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using YukiChan.Utils;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.alias")]
    [StringShortcut("查别名", AllowArguments = true)]
    public async Task<MessageContent> OnAlias(CommandContext ctx, params string[] songname)
    {
        var query = string.Join(' ', songname);

        var resp = await _yukiClient.Arcaea.QuerySongAliases(query);
        if (!resp.Ok)
            return ctx.ReplyServerError(resp);

        return ctx.Reply()
            .Text($"{resp.Data.Name} - {resp.Data.Artist}\n")
            .Text("可用的别名有：\n")
            .Text(string.Join('\n', resp.Data.Aliases));
    }
}