using Konata.Core;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using YukiChan.Core;

namespace YukiChan.Modules;

[Module("Capoo",
    Command = "capoo",
    Description = "随机 Capoo",
    Version = "1.0.0")]
public class CapooModule : ModuleBase
{
    [Command("Capoo",
        Description = "随机 Capoo",
        Usage = "capoo")]
    public static MessageBuilder Capoo(Bot bot, MessageStruct message, string body)
    {
        var images = Directory.GetFiles("Assets/Capoo/");

        if (images.Length == 0)
            return new MessageBuilder()
                .Add(ReplyChain.Create(message))
                .Text("现在没有可用的 Capoo 图呢...");

        var image = images[new Random().Next(images.Length)];

        return new MessageBuilder()
            .Image(image);
    }
}