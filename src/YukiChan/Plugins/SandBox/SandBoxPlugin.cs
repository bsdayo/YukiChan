using Flandre.Core.Messaging;
using Flandre.Framework;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YukiChan.Client.Console;
using YukiChan.Utils;

namespace YukiChan.Plugins.SandBox;

public sealed class SandBoxPlugin : Plugin
{
    private readonly SandBoxService _service;
    private readonly SandBoxPluginOptions _options;
    private readonly FlandreAppOptions _appOptions;
    private readonly ILogger<SandBoxPlugin> _logger;
    private readonly YukiConsoleClient _yukiClient;

    public SandBoxPlugin(SandBoxService service, IOptionsSnapshot<SandBoxPluginOptions> options,
        IOptionsSnapshot<FlandreAppOptions> appOptions, ILogger<SandBoxPlugin> logger,
        YukiConsoleClient yukiClient)
    {
        _service = service;
        _options = options.Value;
        _appOptions = appOptions.Value;
        _logger = logger;
        _yukiClient = yukiClient;
    }

    private bool CheckEnabled(MessageContext ctx)
    {
        if (ctx.Platform != "onebot") return false;
        if (!_options.EnabledChannels.Contains(ctx.GuildId)) return false;
        return true;
    }

    public override async Task OnMessageReceived(MessageContext ctx)
    {
        if (!CheckEnabled(ctx)) return;
        var code = ctx.Message.GetText();
        if (code.StartsWith(_appOptions.CommandPrefix)) return;

        if (ctx.Message.SourceType == MessageSourceType.Channel)
        {
            var resp = await _yukiClient.Guilds.GetAssignee(ctx.Platform, ctx.GuildId!);
            if (!resp.Ok || resp.Data.Assignee != ctx.SelfId) return;
        }

        var result = (await _service.Execute(code, TimeSpan.FromSeconds(10)))?.ToString();

        if (!string.IsNullOrWhiteSpace(result))
            await ctx.Bot.SendMessage(ctx.Message, ctx.Reply(result));
    }

    [Command("sandbox.reset")]
    public MessageContent OnReset(CommandContext ctx)
    {
        if (!CheckEnabled(ctx))
            return ctx.Reply("本群暂未开启沙盒功能哦~");

        _service.Reset();
        _logger.LogInformation("沙盒状态已重置。");
        return ctx.Reply("已重置全局状态。");
    }

    [Command("sandbox.lasterr")]
    public MessageContent OnLastErr(CommandContext ctx)
    {
        if (!CheckEnabled(ctx))
            return ctx.Reply("本群暂未开启沙盒功能哦~");

        if (_service.LastException is null)
            return ctx.Reply("还没有发生错误哦~");

        return ctx.Reply(_service.LastException.Message);
    }
}