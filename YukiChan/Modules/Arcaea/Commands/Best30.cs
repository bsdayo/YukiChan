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
    [Command("Best30",
        Command = "b30",
        Description = "查询 Best30 成绩",
        Usage = "a b30 [名称/好友码]",
        Example = "a b30 ToasterKoishi")]
    public static async Task<MessageBuilder> Best30(Bot bot, MessageStruct message, string body)
    {
        var allSubFlags = new[] { "official" };
        var (args, subFlags) = CommonUtils.ParseCommandBody(body, allSubFlags);

        try
        {
            if (subFlags.Contains("official")) // TODO: ALA Support
                return message.Reply("ArcaeaLimitedAPI 支持正在开发中，请使用默认 API 进行查询。");

            ArcaeaBest30 best30;

            if (args.Length == 0)
            {
                var user = Global.YukiDb.GetArcaeaUser(message.Sender.Uin);
                if (user is null)
                    return message.Reply("请先使用 #a bind <名称/好友码> 绑定您的账号哦~\n")
                        .Text("您也可以使用 #a b30 <名称/好友码> 直接进行查询。");

                await bot.SendReply(message, $"正在查询 {user.Name} 的 Best30 成绩，请耐心等候...");
                Logger.Info(
                    $"正在查询 {message.Sender.Name}({message.Sender.Uin}) -> {user.Name}({user.Id}) 的 Best30 成绩...");

                best30 = ArcaeaBest30.FromAua(await AuaClient.User.Best30(
                    int.Parse(user.Id), 9, AuaReplyWith.SongInfo));
            }
            else
            {
                await bot.SendReply(message, "正在查询该用户的 Best30 成绩，请耐心等候...");
                Logger.Info($"正在查询 {args[0]} 的 Best30 成绩...");

                best30 = ArcaeaBest30.FromAua(await AuaClient.User.Best30(
                    args[0], 9, AuaReplyWith.SongInfo));
            }

            Logger.Info($"正在为 {best30.Name}({best30.Id}) 生成 Best30 图片...");

            var image = await ArcaeaImageGenerator.Best30(best30, AuaClient);

            return new MessageBuilder().Image(image);
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