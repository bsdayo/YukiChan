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

        var cmdRank = Global.YukiDb.GetTodayCommandHistoryRank();

        var mb = new MessageBuilder()
            .Add(ReplyChain.Create(message))
            .Text($"[暮雪酱|YukiChan] {yukiBs.Version}\n")
            .Text($"{yukiBs.Branch}@{yukiBs.CommitHash[..12]}\n")
            .Text($"[Konata.Core] {konataBs.Version}\n")
            .Text($"{konataBs.Branch}@{konataBs.CommitHash[..12]}\n\n")
            //
            .Text($"已加载 {ModuleManager.ModuleCount} 个模块，{ModuleManager.CommandCount} 个指令\n")
            .Text(
                $"接收/处理/发送 - {Global.Information.MessageReceived}/{Global.Information.MessageProcessed + 1}/{Global.Information.MessageSent + 1}\n")
            .Text($"内存占用 - {Process.GetCurrentProcess().WorkingSet64.Bytes2MiB(2)} MiB\n\n")
            //
            .Text("[指令调用排名]\n");

        for (int i = 0, j = 0; i < cmdRank.Length && j < 5; i++, j++)
            mb.Text($"{i + 1}. {cmdRank[i].Key} - {cmdRank[i].Value}次\n");

        return mb.Text("\nMade with love by b1acksoil");
    }
}