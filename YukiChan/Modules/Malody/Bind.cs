using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;
using YukiChan.Utils;

namespace YukiChan.Modules.Malody;

public partial class MalodyModule
{
    [Command("MalodyBind",
        Command = "bind",
        Description = "绑定用户",
        Usage = "ma bind <名称>",
        Example = "ma bind b1acksoil")]
    public static async Task<MessageBuilder> Bind(Bot bot, MessageStruct message, string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return message.Reply("请输入需要绑定的用户名哦~");

        try
        {
            var userList = await MalodyQuery.MalodyUserQuery(body);

            switch (userList.Count)
            {
                case 0:
                    return message.Reply("没有找到该用户哦~");

                case 1:
                    Global.YukiDb.AddMalodyUser(message.Sender.Uin, userList[0].UserId, userList[0].UserName);
                    return message.Reply($"已为您绑定用户 {userList[0].UserName} ({userList[0].UserId})");

                default:
                    if (userList[0].UserName == body)
                    {
                        Global.YukiDb.AddMalodyUser(message.Sender.Uin, userList[0].UserId, userList[0].UserName);
                        return message.Reply($"已为您绑定用户 {userList[0].UserName} ({userList[0].UserId})");
                    }

                    if (userList.Count > 8)
                        return message.Reply("找到的用户过多，请换用更精确的关键词。");
                    var mb = message.Reply("查找到的用户：\n");
                    foreach (var user in userList)
                        mb.Text($"{user.UserName} ({user.UserId})\n");
                    return mb.Text("请换用更精确的关键词重试。");
            }
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