using ArcaeaUnlimitedAPI.Lib.Utils;
using Flandre.Core.Attributes;
using Flandre.Core.Common;
using Flandre.Core.Messaging;
using YukiChan.Utils;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.bind <user: string>")]
    [Option("uncheck", "-u <:bool>")]
    public async Task<MessageContent> OnBind(MessageContext ctx, ParsedArgs args)
    {
        var user = args.GetArgument<string>("user");
        var unc = args.GetOption<bool>("uncheck");

        try
        {
            if (!unc)
            {
                var userdata = await _auaClient.User.Info(user);
                user = userdata.AccountInfo.Name;
                await Global.YukiDb.AddOrUpdateArcaeaUser(ctx.Bot.Platform, ctx.Message.Sender.UserId,
                    userdata.AccountInfo.Code, user);
            }
            else
            {
                await Global.YukiDb.AddOrUpdateArcaeaUser(ctx.Bot.Platform, ctx.Message.Sender.UserId,
                    user, user);
            }

            var reply = $"已成功为您绑定用户：{user}。";

            if (unc) reply += "\n注：绑定前未进行用户检查。若日后查询时发生错误，可以尝试重新绑定。";

            return ctx.Reply(reply);
        }
        catch (AuaException ae)
        {
            return ctx.Reply($"发生了奇怪的错误！({ae.Message})");
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return ctx.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }
}