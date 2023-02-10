using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using Microsoft.Extensions.Logging;
using YukiChan.Shared.Data;
using YukiChan.Shared.Data.Console.Arcaea;
using YukiChan.Utils;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.bind <user: string>")]
    public async Task<MessageContent> OnBind(MessageContext ctx, ParsedArgs args)
    {
        var user = args.GetArgument<string>("user");

        try
        {
            var resp = await _yukiClient.Arcaea.BindUser(
                ctx.Platform, ctx.UserId,
                new ArcaeaBindRequest { BindTarget = user });

            if (resp.Code.IsArcaeaAuaError())
                return ctx.Reply(resp.Message!);
            return resp.Ok ? ctx.Reply($"已成功为您绑定用户：{resp.Data.Name}。") : ctx.ReplyServerError(resp);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred in a.bind");
            return ctx.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }
}