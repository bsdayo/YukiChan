using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;

namespace YukiChan.Modules.Face;

public partial class FaceModule
{
    [Command("Pat",
        Command = "pat",
        StartsWith = "拍",
        Description = "拍~",
        Usage = "拍 <@目标>",
        Example = "拍 @温小朗")]
    public static async Task<MessageBuilder> Pat(Bot bot, MessageStruct message, string body)
    {
        return await GetFromFancyPig(message, body,
            "https://api.iculture.cc/api/face_pat/?QQ=", "拍");
    }
}