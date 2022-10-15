using System.Diagnostics;
using Flandre.Core.Attributes;
using Flandre.Core.Common;
using Flandre.Core.Messaging;
using YukiChan.Utils;

namespace YukiChan.Plugins;

[Plugin("Status")]
public class StatusPlugin : Plugin
{
    public static int MessageReceived;

    public override void OnMessageReceived(MessageContext ctx)
    {
        MessageReceived++;
    }

    [Command("status")]
    public static MessageContent OnStatus(MessageContext ctx)
    {
        var bs = new BuildStamp(typeof(Program));

        return new MessageBuilder()
            .Text($"[暮雪酱|YukiChan] {bs.Version}\n")
            .Text($"{bs.Branch}@{bs.CommitHash[..7]}\n\n")
            .Text($"已接收 {MessageReceived} 条消息\n\n")
            .Text($"内存占用 - {(double)Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024:F2}MiB\n\n")
            .Text("Made with love by b1acksoil.");
    }
}