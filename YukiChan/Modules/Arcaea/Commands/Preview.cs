using ArcaeaUnlimitedAPI.Lib.Models;
using ArcaeaUnlimitedAPI.Lib.Utils;
using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;
using YukiChan.Utils;

// ReSharper disable CheckNamespace
namespace YukiChan.Modules.Arcaea;

public partial class ArcaeaModule
{
    [Command("Preview",
        Command = "preview",
        Description = "获取谱面预览",
        Usage = "a preview <曲名> [难度]",
        Example = "a preview testify byd")]
    public static async Task<MessageBuilder> Preview(Bot bot, MessageStruct message, string body)
    {
        var args = CommonUtils.ParseCommandBody(body);

        if (args.Length == 0)
            return message.Reply("请输入需要获取的曲名哦~");

        var difficulty = args.Length >= 2
            ? ArcaeaUtils.GetRatingClass(args[1])
            : ArcaeaDifficulty.Future;

        if (difficulty is null)
            return message.Reply("请输入正确的难度哦~");

        try
        {
            var songId = ArcaeaSongDatabase.FuzzySearchId(args[0]);

            if (songId is null)
                return message.Reply("没有找到该曲目呢...");

            byte[] preview;
            var cachePath = $"Cache/Arcaea/Preview/{songId}-{difficulty.ToString()!.ToLower()}.jpg";

            try
            {
                preview = await File.ReadAllBytesAsync(cachePath);
            }
            catch
            {
                var qb = new QueryBuilder()
                    .Add("songid", songId)
                    .Add("difficulty", ((int)difficulty).ToString());

                preview = await AuaClient.HttpClient!.GetByteArrayAsync(
                    "assets/preview" + qb.Build());

                await File.WriteAllBytesAsync(cachePath, preview);
                YukiLogger.SaveCache(cachePath);
            }

            return message.Reply().Image(preview);
        }
        catch (AuaException e)
        {
            var errMsg = AuaErrorStatus.GetMessage(e.Status, e.Message);

            return message.Reply(errMsg);
        }
        catch (YukiException e)
        {
            Logger.Error(e);
            return message.Reply(e.Message);
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return message.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }
}