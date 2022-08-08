using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;
using YukiChan.Utils;

namespace YukiChan.Modules;

[Module("Feedback",
    Command = "feedback",
    Description = "反馈意见",
    Version = "1.0.0")]
public class FeedbackModule : ModuleBase
{
    [Command("Feedback",
        Description = "反馈意见",
        Usage = "feedback")]
    public static async Task<MessageBuilder> Feedback(Bot bot, MessageStruct message, string body)
    {
        var mb = new MessageBuilder()
            .Text("- 收到新的反馈\n")
            .Text($"群聊: {message.Receiver.Name} ({message.Receiver.Uin})\n")
            .Text($"用户: {message.Sender.Name} ({message.Sender.Uin})\n")
            .Text($"内容: {body}");

        await bot.SendFriendMessageWithLog("*Master*", uint.Parse(Global.YukiConfig.MasterUin), mb);

        return message.Reply("反馈成功！");
    }
}