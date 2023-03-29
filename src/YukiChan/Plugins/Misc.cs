using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using YukiChan.Utils;

namespace YukiChan.Plugins;

public class MiscPlugin : Plugin
{
    [Command]
    [StringShortcut("帮助")]
    public string Help() => "[暮雪酱帮助文档]\nhttps://yukidocs.sorabs.cc/";

    [Command]
    [StringShortcut("帮我选")]
    public MessageContent Choose(CommandContext ctx, params string[] items)
    {
        var result = items[new Random().Next(items.Length)];
        return ctx.Reply("建议你选择{result}");
    }
}