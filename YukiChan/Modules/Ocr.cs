using Konata.Core;
using Konata.Core.Interfaces.Api;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using YukiChan.Core;
using YukiChan.Utils;

namespace YukiChan.Modules;

[Module("OCR",
    Command = "ocr",
    Description = "图片转文字",
    Version = "1.0.0")]
public class OcrModule : ModuleBase
{
    [Command("Get OCR",
        Description = "图片转文字",
        FallbackCommand = true)]
    public static async Task<MessageBuilder> Ocr(Bot bot, MessageStruct message)
    {
        var imageChain = message.Chain.GetChain<ImageChain>();

        if (imageChain is not null)
            return await GetOcr(bot, message, imageChain);
        else
            bot.GetSession(message, 30, async cbMessage =>
            {
                var cbImageChain = cbMessage.Chain.GetChain<ImageChain>();
                if (cbImageChain is not null)
                    return await GetOcr(bot, message, cbImageChain);

                return cbMessage.Reply("似乎发送了无效的图片呢...");
            });

        return message.Reply("请在 30 秒内发送需要转换的图片哦~");
    }

    private static async Task<MessageBuilder> GetOcr(Bot bot, MessageStruct message, ImageChain imageChain)
    {
        var result = await bot.ImageOcr(imageChain);
        var mrc = new MultiMsgChain();
        var charCount = 0;
        double totalConf = 0;


        var mb = new MessageBuilder("[转换结果]");
        foreach (var ocr in result)
        {
            mb.Text($"\n{ocr.Text}");
            charCount += ocr.Text.Length;
            totalConf += ocr.Confidence;
        }

        mrc.AddMessage(bot.Uin, "源图片", new MessageBuilder()
            .Text("[源图片]")
            .Add(imageChain)
            .Build());
        mrc.AddMessage(bot.Uin, "基本信息", new MessageBuilder()
            .Text("[基本信息]\n")
            .Text($"字符总数: {charCount}\n")
            .Text($"平均准确率: {totalConf / result.Count:N2}")
            .Text($"图片尺寸: {imageChain.Width}x{imageChain.Height}\n")
            .Text($"图片大小: {(double)imageChain.FileLength / 1024 / 1024:N2}\n")
            .Build());
        mrc.AddMessage(bot.Uin, "转换结果", mb.Build());

        return new MessageBuilder(mrc);
    }
}