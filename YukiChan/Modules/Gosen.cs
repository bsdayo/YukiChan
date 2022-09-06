using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;
using YukiChan.Utils;

namespace YukiChan.Modules;

[Module("Gosen",
    Command = "gosen",
    Description = "五千兆生成")]
public class GosenModule : ModuleBase
{
    private static readonly HttpClient Client = new();

    [Command("Gosen",
        Description = "五千兆图片生成",
        FallbackCommand = true)]
    public static async Task<MessageBuilder> OnGosen(Bot bot, MessageStruct message, string body)
    {
        var args = CommonUtils.ParseCommandBody(body);

        var offset = 0;

        switch (args.Length)
        {
            case 0:
                return message.Reply("请输入文本哦~");
            
            case 1:
                return message.Reply("请输入下侧文本哦~");
            
            case >= 3:
                if (int.TryParse(args[2], out var parsed))
                    offset = parsed;
                else return message.Reply("偏移值格式错误~");
                break;
        }

        var image = await Client.GetByteArrayAsync(
            $"{Global.YukiConfig.Gosen.ApiUrl}/?upper={args[0]}&lower={args[1]}&offset={offset}");

        return message.Reply().Image(image);
    }
}