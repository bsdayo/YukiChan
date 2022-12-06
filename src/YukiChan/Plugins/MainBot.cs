using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using Flandre.Framework.Extensions;
using YukiChan.Shared.Database;
using YukiChan.Shared.Utils;

namespace YukiChan.Plugins;

public class MainBotPlugin : Plugin
{
    private readonly YukiDbManager _database;

    public MainBotPlugin(YukiDbManager database) => _database = database;

    [Command("mainbot")]
    public async Task<MessageContent> OnMainBot(CommandContext ctx)
    {
        if (ctx.Message.SourceType != MessageSourceType.Channel || ctx.GuildId is null)
            return ctx.Reply("当前不在群聊环境中哦！");

        ctx.App.SetGuildAssignee(ctx.Platform, ctx.GuildId, ctx.SelfId);
        await _database.UpdateGuildData(ctx.Platform, ctx.GuildId, ctx.SelfId);

        return ctx.Reply("已将本机设置为群内主 Bot~");
    }
}