using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;
using YukiChan.Database.Models;
using YukiChan.Utils;

namespace YukiChan.Modules.Yuki;

public partial class YukiModule
{
    [Command("Set Authority",
        Command = "setauth",
        Authority = YukiUserAuthority.Owner,
        Usage = "yuki setauth <QQ> <权限等级>",
        Example = "yuki setauth 114514 2",
        Description = "设置用户权限")]
    public MessageBuilder SetAuthority(Bot bot, MessageStruct message, string body)
    {
        var args = CommonUtils.ParseCommandBody(body);

        if (args.Length < 2)
            return message.Reply("参数输入有误，请检查输入哦");

        var uinParsed = uint.TryParse(args[0], out var uin);
        var authParsed = int.TryParse(args[1], out var authority);

        if (!(authParsed && authority is > 0 and < 3))
            return message.Reply("权限范围为 0~3，请检查输入~");

        if (!uinParsed)
            return message.Reply("用户账号输入有误，请检查输入~");

        var user = Global.YukiDb.GetUser(uin);

        if (user is null)
            return message.Reply($"用户 {uin} 没有找到...");

        user.Authority = (YukiUserAuthority)authority;

        Global.YukiDb.UpdateUser(user);

        return message.Reply($"用户 {uin} 权限设置成功，当前权限为 {user.Authority.ToString()}");
    }
}