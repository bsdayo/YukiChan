using Konata.Core;
using Konata.Core.Interfaces.Api;
using Konata.Core.Message;
using YukiChan.Core;
using YukiChan.Database.Models;
using YukiChan.Utils;

namespace YukiChan.Modules.Yuki;

public partial class YukiModule
{
    [Command("Dismiss",
        Command = "ban",
        Authority = YukiUserAuthority.Admin,
        Description = "退群")]
    public async Task<MessageBuilder> Dismiss(Bot bot, MessageStruct message, string body)
    {
        var parsed = uint.TryParse(body, out var uin);
        if (!parsed) return message.Reply("输入错误。");

        try
        {
            await bot.GroupLeave(uin);
            return message.Reply($"已成功退出群 {uin}。");
        }
        catch
        {
            return message.Reply($"用户 {body} 没有找到...");
        }
    }
}