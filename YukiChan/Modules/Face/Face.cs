using Konata.Core.Message;
using Konata.Core.Message.Model;
using YukiChan.Core;
using YukiChan.Utils;

namespace YukiChan.Modules.Face;

[Module("Face",
    Command = "face",
    Description = "表情包生成",
    Version = "1.0.0")]
public partial class FaceModule : ModuleBase
{
    private static readonly ModuleLogger Logger = new("Face");

    private static async Task<MessageBuilder?> GetFromFancyPig(MessageStruct message, string body, string apiUrl,
        string prefix)
    {
        var atChain = message.Chain.GetChain<AtChain>();
        var targetUin = body.Replace(prefix, "").Trim();

        if (atChain is null)
        {
            if (!uint.TryParse(targetUin, out _))
                return null;
            
            if (string.IsNullOrWhiteSpace(targetUin))
                return message.Reply("请指定目标哦~");
        }
        
        try
        {
            var image = await NetUtils.DownloadBytes(
                atChain is not null
                    ? apiUrl + atChain.AtUin
                    : apiUrl + targetUin);

            return new MessageBuilder().Image(image);
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return message.Reply($"发生了奇怪的错误呢...({e.Message})");
        }
    }
}