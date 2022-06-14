using Konata.Core;
using Konata.Core.Interfaces.Api;
using Konata.Core.Message;
using YukiChan.Core;

namespace YukiChan.Modules;

[Module("Poke",
    Command = "poke",
    Description = "戳一戳！",
    Version = "1.0.0")]
public class PokeModule : ModuleBase
{
    [Command("Poke",
        Contains = "暮雪暮雪",
        Description = "暮雪戳戳！",
        Usage = "poke")]
    public static MessageBuilder YukiPoke(Bot bot, MessageStruct message)
    {
        switch (message.Type)
        {
            case MessageStruct.SourceType.Group:
                bot.SendGroupPoke(message.Receiver.Uin, message.Sender.Uin);
                break;
            case MessageStruct.SourceType.Friend:
                bot.SendFriendPoke(message.Sender.Uin);
                break;
        }

        return MessageBuilder.Eval("戳戳~");
    }
}