using Konata.Core;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using YukiChan.Core;

namespace YukiChan.Modules;

[Module("Cat",
    Command = "cat",
    Description = "随机猫猫图",
    Version = "1.0.0")]
public class CatModule : ModuleBase
{
    [Command("Cat",
        Contains = "猫猫图",
        Description = "随机猫猫图",
        Usage = "cat")]
    public static MessageBuilder CatCat(Bot bot, MessageStruct message, string body)
    {
        var images = Directory.GetFiles("Assets/Cats/");

        if (images.Length == 0)
            return new MessageBuilder()
                .Add(ReplyChain.Create(message))
                .Text("现在没有可用的猫猫图呢...");

        var image = images[new Random().Next(images.Length)];

        return new MessageBuilder()
            .Image(image);
    }
}