using ArcaeaUnlimitedAPI.Lib.Models;
using ArcaeaUnlimitedAPI.Lib.Utils;
using Flandre.Core.Attributes;
using Flandre.Core.Common;
using Flandre.Core.Messaging;
using Flandre.Core.Messaging.Segments;
using YukiChan.Plugins.Arcaea.Images;
using YukiChan.Plugins.Arcaea.Models;
using YukiChan.Plugins.Arcaea.Models.Database;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.b30 [user: string]")]
    [Option("nya", "-n <:bool>")]
    [Option("dark", "-d <:bool>")]
    [Shortcut("查b30")]
    [Shortcut("查B30")]
    public async Task<MessageContent> OnBest30(MessageContext ctx, ParsedArgs args)
    {
        var userArg = args.GetArgument<string>("user");

        try
        {
            ArcaeaBest30 best30;

            if (string.IsNullOrEmpty(userArg)) // 未提供用户名/好友码
            {
                var user = await Global.YukiDb.GetArcaeaUser(ctx.Bot.Platform, ctx.Message.Sender.UserId);
                if (user is null)
                    return new MessageBuilder()
                        .Text("请先使用 /a bind 名称或好友码 绑定你的账号哦~\n")
                        .Text("你也可以使用 /a b30 名称或好友码 直接查询指定用户。");

                Logger.Info(
                    $"正在查询 {ctx.Message.Sender.Name}({ctx.Message.Sender.UserId}) -> {user.ArcaeaName}({user.ArcaeaId}) 的 Best30 成绩...");
                await ctx.Bot.SendMessage(ctx.Message, $"正在查询 {user.ArcaeaName} 的 Best30 成绩，请耐心等候...");

                best30 = ArcaeaBest30.FromAua(int.TryParse(user.ArcaeaId, out var parsed)
                    ? await _auaClient.User.Best30(parsed, 9, AuaReplyWith.All)
                    : await _auaClient.User.Best30(user.ArcaeaId, 9, AuaReplyWith.All));
            }
            else
            {
                Logger.Info($"正在查询 {userArg} 的 Best30 成绩...");
                await ctx.Bot.SendMessage(ctx.Message, "正在查询该用户的 Best30 成绩，请耐心等候...");
                best30 = ArcaeaBest30.FromAua(
                    await _auaClient.User.Best30(userArg, 9, AuaReplyWith.All));
            }

            Logger.Info($"正在为 {best30.User.Name}({best30.User.Id}) 生成 Best30 图片...");

            var pref = await Global.YukiDb.GetArcaeaUserPreferences(ctx.Bot.Platform, ctx.Message.Sender.UserId)
                       ?? new ArcaeaUserPreferences();
            pref.Dark = pref.Dark || args.GetOption<bool>("dark");
            pref.Nya = pref.Nya || args.GetOption<bool>("nya");

            var image = await ArcaeaImageGenerator.Best30(best30, _auaClient, pref, Logger);

            return new MessageBuilder().Image(ImageSegment.FromData(image));
        }
        catch (Exception e)
        {
            if (e is not AuaException)
                Logger.Error(e);
            return $"发生了奇怪的错误！({e.Message})";
        }
    }
}