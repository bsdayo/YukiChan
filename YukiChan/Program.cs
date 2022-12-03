using System.Runtime.CompilerServices;
using Flandre.Adapters.OneBot;
using Flandre.Framework;
using Flandre.Framework.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Tomlyn;
using YukiChan.Database;
using YukiChan.Plugins;
using YukiChan.Utils;

[assembly: InternalsVisibleTo("YukiChan.Tools")]

namespace YukiChan;

public static class Program
{
    public static void Main(string[] args)
    {
        YukiDir.EnsureExistence();
        var yukiConfig = GetYukiConfig();
        if (args.Contains("--complete-configs"))
            File.WriteAllText($"{YukiDir.Configs}/yuki.toml", Toml.FromModel(yukiConfig));

        var builder = new FlandreAppBuilder(yukiConfig.App);

        // yukiConfig.Plugins.WolframAlpha.FontPath = $"{YukiDir.Assets}/fonts/TitilliumWeb-SemiBold.ttf";

        builder
            // Adapters
            .UseAdapter(new OneBotAdapter(GetOneBotAdapterConfig()))

            // Plugins
            .UseArcaeaPlugin(yukiConfig.Plugins.Arcaea)
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

    public static FlandreApp LoadGuildAssignees(this FlandreApp app)
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