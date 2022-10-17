using ArcaeaUnlimitedAPI.Lib.Models;
using ArcaeaUnlimitedAPI.Lib.Utils;
using Flandre.Core.Attributes;
using Flandre.Core.Common;
using Flandre.Core.Messaging;
using Flandre.Core.Messaging.Segments;
using YukiChan.Plugins.Arcaea.Images;
using YukiChan.Plugins.Arcaea.Models;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("b30 [user: string]")]
    [Option("nya", "-n <:bool>")]
    public async Task<MessageContent> OnBest30(MessageContext ctx, ParsedArgs args)
    {
        var nya = args.GetOption<bool>("nya");
        var userArg = args.GetArgument<string>("user");

        try
        {
            ArcaeaBest30 best30;

            if (string.IsNullOrEmpty(userArg)) // 未提供用户名/好友码
            {
                var user = await Global.YukiDb.GetArcaeaUser(ctx.Bot.Platform, ctx.Message.Sender.UserId);
                if (user is null) return "绑定功能正在开发...";

                Logger.Info(
                    $"正在查询 {ctx.Message.Sender.Name}({ctx.Message.Sender.UserId}) -> {user.ArcaeaName}({user.ArcaeaId}) 的 Best30 成绩...");
                await ctx.Bot.SendMessage(ctx.Message, $"正在查询 {user.ArcaeaName} 的 Best30 成绩，请耐心等候...");

                best30 = ArcaeaBest30.FromAua(
                    await _auaClient.User.Best30(int.Parse(user.ArcaeaId), 9, AuaReplyWith.All));
            }
            else
            {
                Logger.Info($"正在查询 {userArg} 的 Best30 成绩...");
                await ctx.Bot.SendMessage(ctx.Message, "正在查询该用户的 Best30 成绩，请耐心等候...");
                best30 = ArcaeaBest30.FromAua(
                    await _auaClient.User.Best30(userArg, 9, AuaReplyWith.All));
            }

            Logger.Info($"正在为 {best30.User.Name}({best30.User.Id}) 生成 Best30 图片...");

            var image = await ArcaeaImageGenerator.Best30(best30, _auaClient, false, nya, Logger);

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