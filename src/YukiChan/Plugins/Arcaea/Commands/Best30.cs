using ArcaeaUnlimitedAPI.Lib.Models;
using ArcaeaUnlimitedAPI.Lib.Utils;
using Flandre.Core.Messaging;
using Flandre.Core.Messaging.Segments;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using Microsoft.Extensions.Logging;
using YukiChan.Plugins.Arcaea.Images;
using YukiChan.Plugins.Arcaea.Models;
using YukiChan.Shared.Database.Models.Arcaea;
using YukiChan.Shared.Utils;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.b30 [user: string]")]
    [Option("nya", "-n <:bool>")]
    [Option("dark", "-d <:bool>")]
    [Option("official", "-o <:bool>")]
    [Shortcut("查b30")]
    [Shortcut("查B30")]
    public async Task<MessageContent> OnBest30(MessageContext ctx, ParsedArgs args)
    {
        var userArg = args.GetArgument<string>("user");
        var official = args.GetOption<bool>("official");

        try
        {
            ArcaeaBest30 best30;

            if (string.IsNullOrEmpty(userArg)) // 未提供用户名/好友码
            {
                var user = await _database.GetArcaeaUser(ctx.Bot.Platform, ctx.Message.Sender.UserId);
                if (user is null)
                    return ctx.Reply()
                        .Text("请先使用 /a bind 名称或好友码 绑定你的账号哦~\n")
                        .Text("你也可以使用 /a b30 名称或好友码 直接查询指定用户。");

                _logger.LogInformation(
                    "正在使用 {ApiName} 查询 {UserName}({UserId}) -> {ArcaeaName}({ArcaeaId}) 的 Best30 成绩...",
                    official ? "ALA" : "AUA",
                    ctx.Message.Sender.Name, ctx.Message.Sender.UserId,
                    user.ArcaeaName, user.ArcaeaId);

                await ctx.Bot.SendMessage(ctx.Message, official
                    ? $"正在使用官方 API 查询 {user.ArcaeaName} 的 Best30 成绩，请耐心等候..."
                    : $"正在查询 {user.ArcaeaName} 的 Best30 成绩，请耐心等候...");

                // 用户绑定时如果使用 -u (--uncheck) 选项，user.ArcaeaId 的类型不可预料（例如使用名字绑定）
                if (int.TryParse(user.ArcaeaId, out var parsed))
                {
                    if (official)
                    {
                        var usercode = user.ArcaeaId.PadLeft(9, '0');
                        best30 = ArcaeaBest30.FromAla(
                            await _service.AlaClient.User(usercode),
                            await _service.AlaClient.Best30(usercode), usercode);
                    }
                    else
                        best30 = ArcaeaBest30.FromAua(
                            await _service.AuaClient.User.Best30(parsed, 9, AuaReplyWith.All));
                }
                else
                {
                    if (official)
                        return ctx.Reply("官方 API 仅支持好友码绑定，请重新使用好友码绑定后重试。");

                    best30 = ArcaeaBest30.FromAua(
                        await _service.AuaClient.User.Best30(user.ArcaeaId, 9, AuaReplyWith.All));
                }
            }
            else
            {
                _logger.LogInformation("正在查询 {UserName} 的 Best30 成绩...", userArg);
                await ctx.Bot.SendMessage(ctx.Message, "正在查询该用户的 Best30 成绩，请耐心等候...");
                best30 = ArcaeaBest30.FromAua(
                    await _service.AuaClient.User.Best30(userArg, 9, AuaReplyWith.All));

                if (official)
                {
                    if (!int.TryParse(userArg, out var parsed))
                        return ctx.Reply("官方 API 仅支持好友码绑定，请重新使用好友码绑定后重试。");
                    var usercode = parsed.ToString().PadLeft(9, '0');
                    best30 = ArcaeaBest30.FromAla(
                        await _service.AlaClient.User(usercode),
                        await _service.AlaClient.Best30(usercode), usercode);
                }
            }

            _logger.LogInformation("正在为 {ArcaeaName}({ArcaeaId}) 生成 Best30 图片...", best30.User.Name, best30.User.Id);

            var pref = await _database.GetArcaeaUserPreferences(ctx.Bot.Platform, ctx.Message.Sender.UserId)
                       ?? new ArcaeaUserPreferences();
            pref.Dark = pref.Dark || args.GetOption<bool>("dark");
            pref.Nya = pref.Nya || args.GetOption<bool>("nya");

            var image = await ArcaeaImageGenerator.Best30(best30, _service.AuaClient, pref, _logger);

            return ctx.Reply().Image(ImageSegment.FromData(image));
        }
        catch (Exception e)
        {
            if (e is not AuaException)
                _logger.LogError(e, "Error occurred in a.b30");
            return ctx.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }
}