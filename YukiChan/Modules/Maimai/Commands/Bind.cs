using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;
using YukiChan.Utils;

// ReSharper disable CheckNamespace
namespace YukiChan.Modules.Maimai;

public partial class MaimaiModule
{
    [Command("Bind",
        Command = "bind",
        Description = "绑定用户",
        Usage = "mai bind <名称/好友码>",
        Example = "mai bind 114514191")]
    public static MessageBuilder Bind(Bot bot, MessageStruct message, string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return message.Reply("请输入需要绑定的用户名或好友码哦~");

        try
        {
            Global.YukiDb.AddMaimaiUser(message.Sender.Uin, body);

            return message.Reply($"已为您绑定账号 {body}。");
        }
        catch (YukiException e)
        {
            Logger.Error(e);
            return message.Reply(e.Message);
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return message.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }
}