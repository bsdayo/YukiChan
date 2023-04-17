using Flandre.Adapters.OneBot;
using Flandre.Framework;
using Flandre.Plugins.BaiduTranslate;
using Flandre.Plugins.HttpCat;
using Flandre.Plugins.WolframAlpha;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using YukiChan;
using YukiChan.Core;
using YukiChan.Plugins;

var builder = FlandreApp.CreateBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = YukiDir.Root
});

builder.ConfigureInfrastructure(args).ConfigureSerilog();

#region Adapters

builder.Adapters.Add(new OneBotAdapter(
    builder.Configuration.GetSection("Adapters:OneBot").Get<OneBotAdapterConfig>()
    ?? new OneBotAdapterConfig()));

#endregion

#region Plugins

var pluginOpts = builder.Configuration.GetSection("Plugins");

builder.Plugins.AddArcaea(pluginOpts.GetSection("Arcaea"));
builder.Plugins.AddSandBox(pluginOpts.GetSection("SandBox"));
builder.Plugins.AddBaiduTranslate(pluginOpts.GetSection("BaiduTranslate"));
builder.Plugins.AddWolframAlpha(pluginOpts.GetSection("WolframAlpha"));
builder.Plugins.AddHttpCat(pluginOpts.GetSection("HttpCat"));
builder.Plugins.AddGosen(pluginOpts.GetSection("Gosen"));
builder.Plugins.AddAutoAccept(pluginOpts.GetSection("AutoAccept"));
builder.Plugins.Add<ImagesPlugin>();
builder.Plugins.Add<MainBotPlugin>();
builder.Plugins.Add<StatusPlugin>();
builder.Plugins.Add<MiscPlugin>();
builder.Plugins.Add<DebugPlugin>();

builder.CustomizePluginOptions();

#endregion

var app = builder.Build();

#region Middlewares

app.UseQQGroupWarnFilter();
app.UseQQGuildFilter();
app.UseCommandSession();
app.UseCommandParser();
app.UseCommandPrechecker();
app.UseCommandInvoker();

#endregion

app.UpdateArcaeaSongDb();

app.OnReady += (_, _) => StatusPlugin.UpTimeStopwatch.Start();

app.Run();