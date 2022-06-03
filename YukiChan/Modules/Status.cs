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
    public static MessageBuilder ShowStatus(Bot bot, GroupMessageEvent e, string command)
    {
        return new MessageBuilder()
            .Text("[YukiChan]\n")
            .Text($"Processed {Global.Information.MessageProcessed} message(s)\n")
            .Text($"Received {Global.Information.MessageReceived} message(s)\n")
            .Text($"Sent {Global.Information.MessageSent} message(s)\n\n")
            .Text($"GC Memory {GC.GetTotalAllocatedBytes().Bytes2MiB(2)} MiB" +
                  $"({Math.Round((double)GC.GetTotalAllocatedBytes() / GC.GetTotalMemory(false) * 100, 2)}%)\n")
            .Text($"Total Memory {Process.GetCurrentProcess().WorkingSet64.Bytes2MiB(2)} MiB\n\n")
            .Text($"Powered by Konata.Core");
    }
}