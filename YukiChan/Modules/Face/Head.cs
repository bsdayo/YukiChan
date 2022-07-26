using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;

namespace YukiChan.Modules.Face;

public partial class FaceModule
{
    [Command("Head",
        Command = "head",
        StartsWith = "顶",
        Description = "顶~",
        Usage = "顶 <@目标>",
        Example = "顶 @霜霜今天睡多久")]
    public static async Task<MessageBuilder?> Head(Bot bot, MessageStruct message, string body)
    {
        return await GetFromFancyPig(message, body,
            "https://api.iculture.cc/api/face_play/?QQ=", "顶");
    }
}