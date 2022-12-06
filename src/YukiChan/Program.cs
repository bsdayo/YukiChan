using System.Runtime.CompilerServices;
using Flandre.Adapters.OneBot;
using Flandre.Framework;
using Flandre.Framework.Extensions;
using Flandre.Plugins.BaiduTranslate;
using Flandre.Plugins.HttpCat;
using Flandre.Plugins.WolframAlpha;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Tomlyn;
using YukiChan.Plugins;
using YukiChan.Shared;
using YukiChan.Shared.Database;

[assembly: InternalsVisibleTo("YukiChan.Tools")]

namespace YukiChan;

public static class Program
{
    private static readonly YukiConfig YukiConfig = GetYukiConfig();

    public static void Main(string[] args)
    {
        YukiDir.EnsureExistence();
        if (args.Contains("--complete-configs"))
            File.WriteAllText($"{YukiDir.Configs}/yuki.toml", Toml.FromModel(YukiConfig));

        var builder = new FlandreAppBuilder(YukiConfig.App);

        YukiConfig.Plugins.WolframAlpha.FontPath = $"{YukiDir.Assets}/fonts/TitilliumWeb-SemiBold.ttf";

        builder.ConfigureSerilog().AddYukiServices()

            // Adapters
            .UseAdapter(new OneBotAdapter(GetOneBotAdapterConfig()))

            // Plugins
            .UseArcaeaPlugin(YukiConfig.Plugins.Arcaea)
            .UseBaiduTranslatePlugin(YukiConfig.Plugins.BaiduTranslate)
            .UseWolframAlphaPlugin(YukiConfig.Plugins.WolframAlpha)
            .UseHttpCatPlugin(YukiConfig.Plugins.HttpCat)
            .UsePlugin<StatusPlugin>()
            .UsePlugin<ImagesPlugin>()
            .UsePlugin<DebugPlugin>()
            .UsePlugin<MainBotPlugin>()

            // Build FlandreApp
            .Build()

            // Middlewares
            .UseMiddleware(Middlewares.HandleGuildAssignee)
            .UseMiddleware(Middlewares.QqGuildFilter)

            // Load
            .LoadGuildAssignees()

            // Run
            .Run();
    }

    private static FlandreAppBuilder AddYukiServices(this FlandreAppBuilder builder)
    {
        builder.Services.AddSingleton(YukiConfig);
        builder.Services.AddSingleton<YukiDbManager>();
        return builder;
    }

    private static FlandreAppBuilder ConfigureSerilog(this FlandreAppBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File($"{YukiDir.Logs}/yuki/.log", rollingInterval: RollingInterval.Day, shared: true)
            .MinimumLevel.Debug()
            .CreateLogger();

        builder.Services.AddLogging(lb =>
        {
            lb.ClearProviders();
            lb.AddSerilog(dispose: true);
            lb.SetMinimumLevel(YukiConfig.EnableDebugLog ? LogLevel.Debug : LogLevel.Information);
        });
        return builder;
    }

    private static FlandreApp LoadGuildAssignees(this FlandreApp app)
    {
        foreach (var guildData in app.Services.GetRequiredService<YukiDbManager>().GetAllGuildData().Result)
            app.SetGuildAssignee(guildData.Platform, guildData.GuildId, guildData.Assignee);
        return app;
    }

    #region GetConfigs

    public static YukiConfig GetYukiConfig()
    {
        YukiConfig config;
        if (!File.Exists($"{YukiDir.Configs}/yuki.toml"))
        {
            config = new YukiConfig();
            File.WriteAllText($"{YukiDir.Configs}/yuki.toml", Toml.FromModel(config));
        }
        else
        {
            config = Toml.ToModel<YukiConfig>(File.ReadAllText($"{YukiDir.Configs}/yuki.toml"));
        }

        return config;
    }

    public static OneBotAdapterConfig GetOneBotAdapterConfig()
    {
        OneBotAdapterConfig config;
        if (!File.Exists($"{YukiDir.Configs}/onebot.toml"))
        {
            config = new OneBotAdapterConfig();
            File.WriteAllText($"{YukiDir.Configs}/onebot.toml", Toml.FromModel(config));
        }
        else
        {
            config = Toml.ToModel<OneBotAdapterConfig>(File.ReadAllText($"{YukiDir.Configs}/onebot.toml"));
        }

        return config;
    }

    #endregion
}