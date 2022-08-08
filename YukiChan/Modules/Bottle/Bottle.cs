using Konata.Core;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using YukiChan.Core;
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
                return await SaveBottle(message, textChain, imageChain);
            bot.GetSession(message, 30, async cbMessage =>
            {
                try
                {
                    var cbTextChain = cbMessage.Chain.GetChain<TextChain>();
                    var cbImageChain = cbMessage.Chain.GetChain<ImageChain>();

                    if (cbTextChain is not null || cbImageChain is not null)
                        return await SaveBottle(cbMessage, cbTextChain, cbImageChain);

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

    private static async Task<MessageBuilder> SaveBottle(MessageStruct message, TextChain? textChain,
        ImageChain? imageChain)
    {
        string text = "", imageFilename = "";

        if (textChain is not null)
            text = textChain.Content.Trim();
        if (imageChain is not null)
        {
            if (imageChain.FileLength > ImageSizeLimitMb * 1024 * 1024)
                return message.Reply("图片文件过大，请调整后再上传哦~\n")
                    .Text(
                        $"限制: {ImageSizeLimitMb:N4}M   文件: {(double)imageChain.FileLength / 1024 / 1024:N4} M");

            if (imageChain.ImageType == ImageType.Invalid)
                return message.Reply("图片失效啦！");

            imageFilename =
                $"{DateTime.Now.GetTimestamp()}-{message.Receiver.Uin}-{message.Sender.Uin}.{imageChain.ImageType.ToString().ToLower()}";
            var imageData = await NetUtils.DownloadBytes(imageChain.ImageUrl);
            await File.WriteAllBytesAsync($"Data/BottleImages/{imageFilename}", imageData);
        }

        var id = Global.YukiDb.AddBottle(message, text, imageFilename);
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
            var bottle = Global.YukiDb.GetRandomBottle();
            if (bottle is null)
                return message.Reply("当前没有可用的漂流瓶哦~");

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
                .Text("====================\n")
                .Text(bottle.Text);

            return string.IsNullOrWhiteSpace(bottle.ImageFilename)
                ? mb
                : mb.Image(File.ReadAllBytes($"Data/BottleImages/{bottle.ImageFilename}"));
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return message.Reply($"发生了奇怪的错误！({e.Message})");
        }
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
}