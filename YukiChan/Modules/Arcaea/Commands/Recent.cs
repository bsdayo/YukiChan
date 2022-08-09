using ArcaeaUnlimitedAPI.Lib.Models;
using ArcaeaUnlimitedAPI.Lib.Utils;
using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;
using YukiChan.Modules.Arcaea.Images;
using YukiChan.Modules.Arcaea.Models;
using YukiChan.Utils;

// ReSharper disable CheckNamespace
namespace YukiChan.Modules.Arcaea;

public partial class ArcaeaModule
{
    [Command("Recent",
        Command = "recent",
        Shortcut = "查最高",
        Description = "查询最近成绩",
        Usage = "a recent [名称/好友码]",
        Example = "a recent ToasterKoishi",
        FallbackCommand = true)]
    public static async Task<MessageBuilder> Recent(Bot bot, MessageStruct message, string body)
    {
        var args = CommonUtils.ParseCommandBody(body);

        try
        {
            ArcaeaUser userInfo;
            ArcaeaRecord record;

            if (args.Length == 0)
            {
                var user = Global.YukiDb.GetArcaeaUser(message.Sender.Uin);
                if (user is null)
                    return message.Reply("请先使用 #a bind <名称/好友码> 绑定您的账号哦~\n")
                        .Text("您也可以使用 #a recent <名称/好友码> 直接进行查询。");

                Logger.Info(
                    $"正在查询 {message.Sender.Name}({message.Sender.Uin}) -> {user.Name}({user.Id}) 的 Recent 成绩...");

                var resp = await AuaClient.User.Info(user.Id, 1, AuaReplyWith.All);
                if (resp.RecentScore!.Length == 0)
                    return message.Reply("该用户最近没有游玩过曲目呢...");

                userInfo = ArcaeaUser.FromAua(resp.AccountInfo);
                record = ArcaeaRecord.FromAua(resp.RecentScore[0], resp.SongInfo![0]);
            }
            else
            {
                Logger.Info($"正在查询 {args[0]} 的 Recent 成绩...");

                var resp = await AuaClient.User.Info(args[0], 1, AuaReplyWith.All);
                if (resp.RecentScore!.Length == 0)
                    return message.Reply("该用户最近没有游玩过曲目呢...");

                userInfo = ArcaeaUser.FromAua(resp.AccountInfo);
                record = ArcaeaRecord.FromAua(resp.RecentScore[0], resp.SongInfo![0]);
            }

            Logger.Info($"正在为 {userInfo.Name}({userInfo.Id}) 生成 Recent 图片...");

            var image = await ArcaeaImageGenerator.Single(userInfo, record, AuaClient);

            return message.Reply($"{userInfo.Name} ({userInfo.Potential})").Image(image);
        }
        catch (AuaException e)
        {
            Logger.Error(e);

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