using System.Diagnostics;
using Konata.Core;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using YukiChan.Core;
using YukiChan.Utils;

namespace YukiChan.Modules;

[Module("Status",
    Command = "status",
    Description = "查看暮雪酱状态",
    Version = "1.0.0")]
public class StatusModule : ModuleBase
{
    [Command("Status",
        Description = "查看暮雪酱状态",
        Usage = "status",
        Example = "status")]
    public static MessageBuilder ShowStatus(Bot bot, MessageStruct message)
    {
        var konataBs = new BuildStamp { Type = typeof(Bot) };
        var yukiBs = new BuildStamp { Type = typeof(Program) };

        return new MessageBuilder()
            .Add(ReplyChain.Create(message))
            .Text($"[暮雪酱|YukiChan] {yukiBs.Version}\n")
            .Text($"{yukiBs.Branch}@{yukiBs.CommitHash[..12]}\n\n")
            //
            .Text($"[Konata.Core] {konataBs.Version}\n")
            .Text($"{konataBs.Branch}@{konataBs.CommitHash[..12]}\n\n")
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