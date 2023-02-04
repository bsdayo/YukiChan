using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using YukiChan.Utils;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.alias <songname:text>")]
    [Shortcut("查别名")]
    public async Task<MessageContent> OnAlias(MessageContext ctx, ParsedArgs args)
    {
        var query = args.GetArgument<string>("songname");

        var resp = await _yukiClient.Arcaea.QuerySongAliases(query);
        if (!resp.Ok)
            return ctx.ReplyServerError(resp);

        return ctx.Reply()
            .Text($"{resp.Data.Name} - {resp.Data.Artist}\n")
            .Text("可用的别名有：\n")
            .Text(string.Join('\n', resp.Data.Aliases));
    }
}