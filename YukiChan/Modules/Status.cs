using System;
using System.Diagnostics;
using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Message;
using YukiChan.Attributes;
using YukiChan.Core;
using YukiChan.Models;
using YukiChan.Utils;

namespace YukiChan.Modules;

[Module("Status",
    Command = "status",
    Description = "Show bot status.",
    Version = "1.0.0")]
public class StatusModule : ModuleBase
{
    [Command("Status",
        Description = "Show bot status.",
        Example = "status")]
    public static MessageBuilder ShowStatus()
    {
        return new MessageBuilder()
            .Text("[暮雪酱|YukiChan]\n\n")
            //
            .Text($"[Konata.Core] {BuildStamp.Version}\n")
            .Text($"{BuildStamp.Branch}@{BuildStamp.CommitHash[..12]}\n\n")
            //
            .Text($"已处理 {Global.Information.MessageProcessed} 条消息\n")
            .Text($"已接收 {Global.Information.MessageReceived} 条消息\n")
            .Text($"已发送 {Global.Information.MessageSent} 条消息\n\n")
            //
            .Text($"GC 内存 {GC.GetTotalAllocatedBytes().Bytes2MiB(2)} MiB " +
                  $"({Math.Round((double)GC.GetTotalAllocatedBytes() / GC.GetTotalMemory(false) * 100, 2)}%)\n")
            .Text($"总内存 {Process.GetCurrentProcess().WorkingSet64.Bytes2MiB(2)} MiB\n\n")
            //
            .Text("Made with love by b1acksoil");
    }
}