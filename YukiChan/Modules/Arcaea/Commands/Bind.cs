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
            BotLogger.Error(e);

            return message.Reply(e.Status switch
            {
                -1 => "用户名或好友码输入错误。",
                -2 => "好友码输入错误，请检查格式。",
                -3 => "用户未找到，请检查是否输入有误。\n如果您使用用户名绑定，可以更换为好友码重试。",
                -4 => "查询到的用户过多，请换用更为精确的名称或好友码绑定。",
                _ => $"请求用户信息时发生了错误。\n({e.Status}: {e.Message})"
            });
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