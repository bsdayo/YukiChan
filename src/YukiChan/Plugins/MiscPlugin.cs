using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;

namespace YukiChan.Plugins;

public class MiscPlugin : Plugin
{
    [Command("help")]
    [StringShortcut("帮助")]
    public MessageContent OnHelp() =>
        "[暮雪酱帮助文档]\nhttps://yukidocs.sorabs.cc/";
}