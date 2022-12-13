using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using Microsoft.Extensions.Logging;
using YukiChan.Shared.Utils;

namespace YukiChan.Plugins.SandBox;

public sealed class SandBoxPlugin : Plugin
{
    private readonly SandBoxService _service;

    private readonly SandBoxPluginConfig _config;

    private readonly YukiConfig _yukiConfig;

    private readonly ILogger<SandBoxPlugin> _logger;

    public SandBoxPlugin(SandBoxService service, SandBoxPluginConfig config, YukiConfig yukiConfig,
        ILogger<SandBoxPlugin> logger)
    {
        _service = service;
        _config = config;
        _yukiConfig = yukiConfig;
        _logger = logger;
    }

    private bool CheckEnabled(MessageContext ctx)
    {
        if (ctx.Platform != "onebot") return false;
        if (!_config.EnabledGroups.Contains(ctx.GuildId)) return false;
        return true;
    }

    public override async Task OnMessageReceived(MessageContext ctx)
    {
        if (!CheckEnabled(ctx)) return;

        var code = ctx.Message.GetText();
        if (code.StartsWith(_yukiConfig.App.CommandPrefix)) return;

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