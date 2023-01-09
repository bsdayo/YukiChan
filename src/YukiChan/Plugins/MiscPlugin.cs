using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;

namespace YukiChan.Plugins;

public class MiscPlugin : Plugin
{
    [Command("help")]
    [Shortcut("帮助")]
    public MessageContent OnHelp() =>
        "[暮雪酱帮助文档]\nhttps://docs.sorabs.cc/YukiChan/";
}