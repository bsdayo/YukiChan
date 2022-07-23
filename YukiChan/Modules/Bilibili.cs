using BiliSharp;
using BiliSharp.Utils;
using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;
using YukiChan.Utils;

namespace YukiChan.Modules;

[Module("Bilibili",
    Command = "bilibili",
    Description = "哔哩哔哩相关功能",
    Version = "1.0.0")]
public class BilibiliModule : ModuleBase
{
    private static readonly ModuleLogger Logger = new("Bilibili");

    [Command("Fetch Info From AV Code",
        Command = "av",
        StartsWith = "av",
        Description = "通过 AV 号获取B站视频信息",
        Usage = "bilibili av <bvcode>",
        Example = "bilibili av av170001")]
    public static async Task<MessageBuilder> GetCoverFromAv(Bot bot, MessageStruct message, string body)
    {
        try
        {
            long.TryParse(body[2..], out var avid);
            if (avid == 0) return null!;
            return await ConstructInfoMessage(message, new BiliVideo(avid));
        }
        catch (BiliException exception)
        {
            Logger.Error(exception);
            return MessageBuilder.Eval("视频解析失败: " + exception.Message);
        }
        catch (Exception exception)
        {
            Logger.Error(exception);
            return MessageBuilder.Eval("发生了奇怪的错误... " + exception.Message);
        }
    }

    [Command("Fetch Cover From BV Code",
        Command = "bv",
        StartsWith = "BV",
        Description = "通过 BV 号获取B站封面视频图片",
        Usage = "bilibili bv <bvcode>",
        Example = "bilibili bv BV1Js411o76u")]
    public static async Task<MessageBuilder> GetCoverFromBv(Bot bot, MessageStruct message, string body)
    {
        try
        {
            return await ConstructInfoMessage(message, new BiliVideo(body));
        }
        catch (BiliException exception)
        {
            Logger.Error(exception);
            return MessageBuilder.Eval("视频解析失败: " + exception.Message);
        }
        catch (Exception exception)
        {
            Logger.Error(exception);
            return MessageBuilder.Eval("发生了奇怪的错误... " + exception.Message);
        }
    }

    private static async Task<MessageBuilder> ConstructInfoMessage(MessageStruct message, BiliVideo video)
    {
        var info = await video.GetInfoAsync();
        var cover = await NetUtils.DownloadBytes(info.PictureUrl);
        return message.Reply()
            .Image(cover)
            .Text($"{info.Title}\n")
            .Text($"by {info.Owner.Name}\n")
            .Text($"时长 / {new TimeSpan(0, 0, info.Duration).ToString("c").Split(".")[0]}\n")
            .Text($"播放 / {info.Stat.View}   ")
            .Text($"点赞 / {info.Stat.Like}\n")
            .Text($"投币 / {info.Stat.Coin}   ")
            .Text($"收藏 / {info.Stat.Favorite}\n")
            .Text($"弹幕 / {info.Stat.Danmaku}   ")
            .Text($"分享 / {info.Stat.Share}\n")
            .Text($"简介 / {info.Description}\n")
            .Text($"发布时间：{CommonUtils.FormatTimestamp(info.PublishTime)}");
    }
}