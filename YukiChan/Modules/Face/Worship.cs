using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;

namespace YukiChan.Modules.Face;

public partial class FaceModule
{
    [Command("Worship",
        Command = "worship",
        StartsWith = "拜",
        Description = "拜~",
        Usage = "拜 <@目标>",
        Example = "拜 @玲")]
    public static async Task<MessageBuilder?> Worship(Bot bot, MessageStruct message, string body)
    {
        return await GetFromFancyPig(message, body,
            "https://api.iculture.cc/api/face_worship/?QQ=", "拜");
    }
}