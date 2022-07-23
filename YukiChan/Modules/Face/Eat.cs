using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;

namespace YukiChan.Modules.Face;

public partial class FaceModule
{
    [Command("Eat",
        Command = "eat",
        StartsWith = "吃",
        Description = "吃~",
        Usage = "吃 <@目标>",
        Example = "吃 @本子今天又摸了一个早上")]
    public static async Task<MessageBuilder> Eat(Bot bot, MessageStruct message, string body)
    {
        return await GetFromFancyPig(message, body,
            "https://api.iculture.cc/api/chi/?QQ=", "吃");
    }
}