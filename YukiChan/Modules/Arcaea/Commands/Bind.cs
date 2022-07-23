using ArcaeaUnlimitedAPI.Lib.Utils;
using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;
using YukiChan.Utils;

// ReSharper disable CheckNamespace
namespace YukiChan.Modules.Arcaea;

public partial class ArcaeaModule
{
    [Command("Bind",
        Command = "bind",
        Description = "绑定用户",
        Usage = "a bind <名称/好友码>",
        Example = "a bind 114514191")]
    public static async Task<MessageBuilder> Bind(Bot bot, MessageStruct message, string body)
    {
        var username = body.Trim();

        if (username == "")
            return message.Reply("请输入需要绑定的用户名或好友码哦~");

        try
        {
            var userInfo = await AuaClient.User.Info(username);

            Global.YukiDb.AddArcaeaUser(
                message.Sender.Uin, userInfo.AccountInfo.Code, userInfo.AccountInfo.Name);

            var rating = (double)userInfo.AccountInfo.Rating / 100;

            return message.Reply()
                .Text("绑定成功！\n")
                .Text($"{userInfo.AccountInfo.Name} ({(rating >= 0 ? rating : "--")})\n")
                .Text($"注册时间: {CommonUtils.FormatTimestamp(userInfo.AccountInfo.JoinDate, true)}");
        }
        catch (AuaException e)
        {
            Logger.Error(e);

            var errMsg = AuaErrorStatus.GetMessage(e.Status, e.Message);

            errMsg = e.Status switch
            {
                -3 => $"{errMsg}\n如果您使用用户名绑定，可以更换为好友码重试。",
                -4 => $"{errMsg[..^1]}，请换用更为精确的名称或好友码绑定。",
                _ => errMsg
            };

            return message.Reply(errMsg);
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