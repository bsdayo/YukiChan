using Konata.Core;
using Konata.Core.Interfaces.Api;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using YukiChan.Core;
using YukiChan.Database.Models;
using YukiChan.Utils;

namespace YukiChan.Modules.Bottle;

[Module("Bottle",
    Command = "bottle",
    Description = "漂流瓶",
    Version = "1.0.0")]
public class BottleModule : ModuleBase
{
    private static readonly ModuleLogger Logger = new("Bottle");
    private const double ImageSizeLimitMb = 4;

    [Command("ThrowBottle",
        Command = "throw",
        Shortcut = "扔漂流瓶",
        Usage = "bottle throw [内容]",
        Example = "bottle throw 616sb!")]
    public static async Task<MessageBuilder?> ThrowBottle(Bot bot, MessageStruct message, string body)
    {
        try
        {
            var textChain = string.IsNullOrWhiteSpace(body) ? null : TextChain.Create(body);
            var imageChain = message.Chain.GetChain<ImageChain>();

            if (textChain is not null || imageChain is not null)
                return await SaveBottle(bot, message, textChain, imageChain);
            bot.GetSession(message, 30, async cbMessage =>
            {
                try
                {
                    var cbTextChain = cbMessage.Chain.GetChain<TextChain>();
                    var cbImageChain = cbMessage.Chain.GetChain<ImageChain>();

                    if (cbTextChain is not null || cbImageChain is not null)
                        return await SaveBottle(bot, cbMessage, cbTextChain, cbImageChain);

                    return cbMessage.Reply("似乎输入了无效的漂流瓶内容呢...");
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return message.Reply($"发生了奇怪的错误！({e.Message})");
                }
            });

            return message.Reply("请在 30 秒内发送漂流瓶内容哦~");
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return message.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }

    private static async Task<MessageBuilder> SaveBottle(Bot bot, MessageStruct message, TextChain? textChain,
        ImageChain? imageChain)
    {
        var text = "";
        int id;

        if (textChain is not null)
            text = textChain.Content.Trim();
        if (text.HasSensitiveWords())
            return message.Reply("不可以输入敏感词汇哦！");

        if (imageChain is not null)
        {
            if (imageChain.FileLength > ImageSizeLimitMb * 1024 * 1024)
                return message.Reply("图片文件过大，请调整后再上传哦~\n")
                    .Text(
                        $"限制: {ImageSizeLimitMb:N4}M   文件: {(double)imageChain.FileLength / 1024 / 1024:N4} M");

            if (imageChain.ImageType == ImageType.Invalid)
                return message.Reply("图片失效啦！");

            // 检测成绩图
            var ocrResult = await bot.ImageOcr(imageChain);
            var words = new List<string>();
            var bannedWords = "far,lost,score,max recall,past,present,future".Split(",");
            foreach (var ocr in ocrResult)
            foreach (var word in bannedWords)
                if (ocr.Text.ToLower().Contains(word))
                    words.Add(word);
            if ((words.Contains("max recall") && words.Contains("score") && words.Contains("far") &&
                 words.Contains("lost")) ||
                (words.Contains("past") && words.Contains("present") && words.Contains("future")))
                return message.Reply("不可以发送成绩图哦~");


            var extName = imageChain.ImageType switch
            {
                0 => "jpg",
                ImageType.Apng => "png",
                ImageType.Face => "jpg",
                ImageType.Pjpeg => "jpg",
                ImageType.Sharpp => "webp",
                _ => imageChain.ImageType.ToString().ToLower()
            };

            var bottle = Global.YukiDb.AddBottle(message, text, "");
            id = bottle.Id;
            bottle.ImageFilename = $"{bottle.Id}.{extName}";
            var imageData = await NetUtils.DownloadBytes(imageChain.ImageUrl);
            await File.WriteAllBytesAsync($"Data/BottleImages/{bottle.ImageFilename}", imageData);

            Global.YukiDb.UpdateBottle(bottle);
        }
        else id = Global.YukiDb.AddBottle(message, text, "").Id;

        return message.Reply($"漂流瓶 {id} 号开始漂流啦！")
            .Text($"可以随时使用 #bottle cancel {id} 召回哦~");
    }

    [Command("PickBottle",
        Command = "pick",
        Shortcut = "捡漂流瓶",
        Usage = "bottle pick")]
    public static MessageBuilder PickBottle(Bot bot, MessageStruct message)
    {
        try
        {
            // Global.YukiDb.FixBottle();
            var bottle = Global.YukiDb.GetRandomBottle();
            if (bottle is null)
                return message.Reply("当前没有可用的漂流瓶哦~");

            return GetBottleInfo(message, bottle);
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return message.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }

    private static MessageBuilder GetBottleInfo(MessageStruct message, Bottle bottle)
    {
        var days = new TimeSpan(0, 0, 0,
            (int)(DateTime.Now.GetTimestamp() - bottle.Timestamp)).Days;

        var mb = message.Reply()
            .Text($"[漂流瓶 No.{bottle.Id}]\n")
            .Text($"在 {bottle.Timestamp.FormatTimestamp()}，\n");

        if (bottle.Context == MessageStruct.SourceType.Group)
            mb
                .Text($"由 {bottle.UserName} 投掷于\n")
                .Text($"    {bottle.GroupName}，\n");
        else
            mb.Text($"由 {bottle.UserUin} 投掷于私聊，\n");

        mb
            .Text(days == 0 ? "今天开始漂流的哦！\n" : $"已经漂流 {days} 天啦！\n")
            .Text("-------------------------\n")
            .Text(bottle.Text);

        return string.IsNullOrWhiteSpace(bottle.ImageFilename)
            ? mb
            : mb.Image(File.ReadAllBytes($"Data/BottleImages/{bottle.ImageFilename}"));
    }

    [Command("CancelBottle",
        Command = "cancel",
        Usage = "bottle cancel <ID>",
        Example = "bottle cancel 11451")]
    public static MessageBuilder CancelBottle(Bot bot, MessageStruct message, string body)
    {
        var success = int.TryParse(body, out var bottleId);
        if (!success)
            return message.Reply("输入了无效的 ID 哦！");

        var bottle = Global.YukiDb.GetBottle(bottleId);
        if (bottle is null)
            return message.Reply("没有找到这个漂流瓶呢...");
        if (bottle.UserUin != message.Sender.Uin)
            return message.Reply("这个漂流瓶不是你投掷的哦！");

        if (!string.IsNullOrWhiteSpace(bottle.ImageFilename))
            File.Delete($"Data/BottleImages/{bottle.ImageFilename}");
        Global.YukiDb.RemoveBottle(bottleId);

        return message.Reply("漂流瓶已经成功召回了哦~");
    }

    [Command("RemoveBottle",
        Command = "remove",
        Authority = YukiUserAuthority.Owner,
        Hidden = true)]
    public static MessageBuilder RemoveBottle(Bot bot, MessageStruct message, string body)
    {
        var success = int.TryParse(body, out var bottleId);
        if (!success)
            return message.Reply("输入了无效的 ID 哦！");

        var bottle = Global.YukiDb.GetBottle(bottleId);
        if (bottle is null)
            return message.Reply("没有找到这个漂流瓶呢...");

        if (!string.IsNullOrWhiteSpace(bottle.ImageFilename))
            File.Delete($"Data/BottleImages/{bottle.ImageFilename}");
        Global.YukiDb.RemoveBottle(bottleId);

        return message.Reply("漂流瓶已经成功移除了哦~");
    }

    [Command("ViewBottle",
        Command = "view",
        Authority = YukiUserAuthority.Owner,
        Hidden = true)]
    public static MessageBuilder ViewBottle(Bot bot, MessageStruct message, string body)
    {
        var success = int.TryParse(body, out var bottleId);
        if (!success)
            return message.Reply("输入了无效的 ID 哦！");

        var bottle = Global.YukiDb.GetBottle(bottleId);
        if (bottle is null)
            return message.Reply("没有找到这个漂流瓶呢...");

        return GetBottleInfo(message, bottle);
    }
}