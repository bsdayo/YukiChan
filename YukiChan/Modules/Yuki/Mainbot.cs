using Konata.Core;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using YukiChan.Core;
using YukiChan.Database.Models;
using YukiChan.Utils;

namespace YukiChan.Modules.Yuki;

public partial class YukiModule
{
    [Command("Mainbot",
        Command = "mainbot",
        Description = "切换群内主 Bot")]
    public MessageBuilder? OnMainbot(Bot bot, MessageStruct message)
    {
        var atChain = message.Chain.GetChain<AtChain>();
        if (atChain is not null && atChain.AtUin == bot.Uin)
        {
            Global.YukiDb.UpdateGroupAssignee(message.Receiver.Uin, bot.Uin);
            return message.Reply($"已切换为群内主bot。");
        }

        return null;
    }
}