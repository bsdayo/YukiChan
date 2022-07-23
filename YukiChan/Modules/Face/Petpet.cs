using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;

namespace YukiChan.Modules.Face;

public partial class FaceModule
{
    [Command("Petpet",
        Command = "petpet",
        StartsWith = "摸",
        Description = "摸~",
        Usage = "摸 <@目标>",
        Example = "摸 @下午好")]
    public static async Task<MessageBuilder> Petpet(Bot bot, MessageStruct message, string body)
    {
        return await GetFromFancyPig(message, body,
            "https://api.iculture.cc/api/face_petpet/?QQ=", "摸");
    }
}