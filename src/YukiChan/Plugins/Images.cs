using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using YukiChan.Utils;

namespace YukiChan.Plugins;

public class ImagesPlugin : Plugin
{
    [Command("cat")]
    public static MessageContent OnCat()
    {
        var images = Directory.GetFiles(YukiDir.CatAssets);

        if (images.Length == 0)
            return "现在没有可用的猫猫图呢...";

        var image = File.ReadAllBytes(images[new Random().Next(images.Length)]);

        return new MessageBuilder().Image(image);
    }

    [Command("capoo")]
    public static MessageContent OnCapoo()
    {
        var images = Directory.GetFiles(YukiDir.CapooAssets);

        if (images.Length == 0)
            return "现在没有可用的 capoo 呢...";

        var image = File.ReadAllBytes(images[new Random().Next(images.Length)]);

        return new MessageBuilder().Image(image);
    }
}