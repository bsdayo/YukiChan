using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;

namespace YukiChan.Modules.Face;

public partial class FaceModule
{
    [Command("Pound",
        Command = "pound",
        StartsWith = "捣",
        Description = "捣~",
        Usage = "捣 <@目标>",
        Example = "捣 @每天手冲0.79次")]
    public static async Task<MessageBuilder?> Pound(Bot bot, MessageStruct message, string body)
    {
        return await GetFromFancyPig(message, body,
            "https://api.iculture.cc/api/face_pound/?QQ=", "捣");
    }
}