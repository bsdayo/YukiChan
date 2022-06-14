using Konata.Core;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using YukiChan.Core;
using YukiChan.Database.Models;

namespace YukiChan.Modules.Yuki;

public partial class YukiModule
{
    [Command("Whoami",
        Command = "whoami",
        Authority = YukiUserAuthority.Banned,
        Description = "查看个人权限")]
    public static MessageBuilder Whoami(Bot bot, MessageStruct message)
    {
        var user = Global.YukiDb.GetUser(message.Sender.Uin);
        var authority = user?.Authority ?? YukiUserAuthority.User;

        return new MessageBuilder()
            .Add(ReplyChain.Create(message))
            .Text($"你的用户权限为 {authority.ToString()}");
    }
}