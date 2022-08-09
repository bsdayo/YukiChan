using ArcaeaUnlimitedAPI.Lib.Models;
using ArcaeaUnlimitedAPI.Lib.Responses;
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
    [Command("Best",
        Command = "best",
        Shortcut = "查最高",
        Description = "查询单曲最高分",
        Usage = "a best <曲名> [难度]",
        Example = "a best testify byd")]
    public static async Task<MessageBuilder> Best(Bot bot, MessageStruct message, string body)
    {
        var args = CommonUtils.ParseCommandBody(body);

        try
        {
            AuaUserBestContent content;
            ArcaeaDifficulty? difficulty;
            ArcaeaDatabaseUser? dbUser;

            switch (args.Length)
            {
                case 0:
                    return message.Reply("请输入需要查询的曲目哦~");

                case 1:
                    dbUser = Global.YukiDb.GetArcaeaUser(message.Sender.Uin);
                    if (dbUser is null)
                        return message.Reply("请先使用 #a bind <名称/好友码> 绑定您的账号哦~\n")
                            .Text("您也可以使用 #a best <名称/好友码> <曲名> [难度] 直接进行查询。");

                    Logger.Info(
                        $"正在查询 {message.Sender.Name}({message.Sender.Uin}) -> {dbUser.Name}({dbUser.Id}) 的 {args[0]} [Future] 最高分...");

                    content = await AuaClient.User.Best(dbUser.Id, args[0], ArcaeaDifficulty.Future, AuaReplyWith.All);
                    break;

                case 2:
                    difficulty = ArcaeaUtils.GetRatingClass(args[1]);
                    if (difficulty is not null)
                    {
                        dbUser = Global.YukiDb.GetArcaeaUser(message.Sender.Uin);
                        if (dbUser is null)
                            return message.Reply("请先使用 #a bind <名称/好友码> 绑定您的账号哦~\n")
                                .Text("您也可以使用 #a best <名称/好友码> <曲名> [难度] 直接进行查询。");
                        Logger.Info(
                            $"正在查询 {message.Sender.Name}({message.Sender.Uin}) -> {dbUser.Name}({dbUser.Id}) 的 {args[0]} [{difficulty}] 最高分...");
                        content = await AuaClient.User.Best(dbUser.Id, args[0], difficulty.Value, AuaReplyWith.All);
                    }
                    else
                    {
                        Logger.Info($"正在查询 {args[0]} 的 {args[1]} [Future] 最高分...");
                        content = await AuaClient.User.Best(args[0], args[1], ArcaeaDifficulty.Future,
                            AuaReplyWith.All);
                    }

                    break;

                default:
                    difficulty = ArcaeaUtils.GetRatingClass(args[2]);
                    if (difficulty is null)
                        return message.Reply("请输入正确的难度格式哦~");
                    Logger.Info($"正在查询 {args[0]} 的 {args[1]} [{difficulty}] 最高分...");
                    content = await AuaClient.User.Best(args[0], args[1], difficulty.Value, AuaReplyWith.All);
                    break;
            }

            var user = ArcaeaUser.FromAua(content.AccountInfo);
            var record = ArcaeaRecord.FromAua(content.Record, content.SongInfo![0]);

            Logger.Info($"正在为 {user.Name}({user.Id}) 生成 Recent 图片...");

            var image = await ArcaeaImageGenerator.Single(user, record, AuaClient);

            return message.Reply($"{user.Name} ({user.Potential})").Image(image);
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