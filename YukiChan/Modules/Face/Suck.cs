using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;

namespace YukiChan.Modules.Face;

public partial class FaceModule
{
    [Command("Suck",
        Command = "suck",
        StartsWith = "吸",
        Description = "吸~",
        Usage = "吸 <@目标>",
        Example = "吸 @bs睡醒了吗")]
    public static async Task<MessageBuilder> Suck(Bot bot, MessageStruct message, string body)
    {
        return await GetFromFancyPig(message, body,
            "https://api.iculture.cc/api/face_suck/?QQ=", "吸");
    }
}