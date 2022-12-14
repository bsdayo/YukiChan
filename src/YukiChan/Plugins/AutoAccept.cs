using Flandre.Core.Common;
using Flandre.Core.Events;
using Flandre.Framework.Common;
using Microsoft.Extensions.Logging;

namespace YukiChan.Plugins;

public sealed class AutoAcceptPlugin : Plugin
{
    private readonly AutoAcceptPluginConfig _config;

    private readonly ILogger<AutoAcceptPlugin> _logger;

    public AutoAcceptPlugin(AutoAcceptPluginConfig config, ILogger<AutoAcceptPlugin> logger)
        => (_config, _logger) = (config, logger);

    public override async Task OnFriendRequested(Context ctx, BotFriendRequestedEvent e)
    {
        if (!_config.BotIds.Contains(ctx.SelfId)) return;
        await ctx.Bot.HandleFriendRequest(e, true);
        _logger.LogInformation("已接受来自 {UserName} ({UserId}) 的好友申请。",
            e.RequesterName, e.RequesterId);
    }

    public override async Task OnGuildInvited(Context ctx, BotGuildInvitedEvent e)
    {
        if (!_config.BotIds.Contains(ctx.SelfId)) return;
        await ctx.Bot.HandleGuildInvitation(e, true);
        _logger.LogInformation("已接受来自 {UserName} ({UserId}) 的邀请，进入群 {GuildName} ({GuildId})。",
            e.InviterName, e.InviterId, e.GuildName, e.GuildId);
    }
}

public sealed class AutoAcceptPluginConfig
{
    public string[] BotIds { get; set; } = Array.Empty<string>();
}