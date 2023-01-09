using Flandre.Framework;
using Flandre.Plugins.BaiduTranslate;
using Flandre.Plugins.HttpCat;
using Flandre.Plugins.WolframAlpha;
using Microsoft.Extensions.Hosting;
using YukiChan;
using YukiChan.Plugins;
using YukiChan.Shared;

var builder = FlandreApp.CreateBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = YukiDir.Root
});
var pluginOpts = builder.Configuration.GetSection("Plugins");

using var app = builder.ConfigureInfrastructure(args).ConfigureSerilog()
    // Adapters
    .AddOneBotAdapter()

    // Plugins
    .AddArcaeaPlugin(pluginOpts.GetSection("Arcaea"))
    .AddSandBoxPlugin(pluginOpts.GetSection("SandBox"))
    .AddBaiduTranslatePlugin(pluginOpts.GetSection("BaiduTranslate"))
    .AddWolframAlphaPlugin(pluginOpts.GetSection("WolframAlpha"))
    .AddHttpCatPlugin(pluginOpts.GetSection("HttpCat"))
    .AddGosenPlugin(pluginOpts.GetSection("Gosen"))
    .AddAutoAcceptPlugin(pluginOpts.GetSection("AutoAccept"))
    .AddPlugin<ImagesPlugin>()
    .AddPlugin<MainBotPlugin>()
    .AddPlugin<StatusPlugin>()
    .AddPlugin<MiscPlugin>()
    .AddPlugin<DebugPlugin>()
    .CustomizePluginOptions()

    // Build FlandreApp
    .Build()

    // Middlewares
    .UseMiddleware(Middlewares.QqGroupWarnFilter)
    .UseMiddleware(Middlewares.QqGuildFilter)
    .UseMiddleware(Middlewares.HandleGuildAssignee)

    // Load
    .LoadGuildAssignees();

// Events
app.OnCommandInvoking += YukiEventHandlers.OnCommandInvoking;
app.OnCommandInvoked += YukiEventHandlers.OnCommandInvoked;

app.Run();