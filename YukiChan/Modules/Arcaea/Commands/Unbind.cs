using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;
using YukiChan.Utils;

// ReSharper disable CheckNamespace
namespace YukiChan.Modules.Arcaea;

public partial class ArcaeaModule
{
    [Command("Unbind",
        Command = "unbind",
        Description = "解绑用户",
        Usage = "a unbind",
        Example = "a unbind")]
    public static MessageBuilder Unbind(Bot bot, MessageStruct message)
    {
        try
        {
            return message.Reply(
                Global.YukiDb.DeleteArcaeaUser(message.Sender.Uin)
                    ? "解绑成功！"
                    : "您还没有绑定一个用户哦~");
        }
        catch (YukiException e)
        {
            BotLogger.Error(e);
            return message.Reply(e.Message);
        }
        catch (Exception e)
        {
            BotLogger.Error(e);
            return message.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }
}