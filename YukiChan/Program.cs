using Flandre.Adapters.Konata;
using Flandre.Adapters.OneBot;
using Flandre.Core;
using Flandre.Core.Utils;
using Flandre.Plugins.HttpCat;
using Konata.Core.Common;
using Tomlyn;
using YukiChan.Plugins;
using YukiChan.Plugins.Arcaea;
using YukiChan.Utils;

namespace YukiChan;

public static class Program
{
    public static void Main(string[] args)
    {
        YukiDir.EnsureExistence();
        Logger.DefaultLoggingHandlers.Add(LoggerExtensions.SaveToFile);

        var yukiConfig = GetYukiConfig();
        var konataConfig = GetKonataAdapterConfig();
        Global.YukiConfig = yukiConfig;

        var app = new FlandreApp(yukiConfig.App);

        // Update Konata config
        app.OnAppReady += (_, _) =>
            File.WriteAllText($"{YukiDir.Configs}/konata.toml", Toml.FromModel(konataConfig));

        if (args.Contains("--complete-configs"))
            File.WriteAllText($"{YukiDir.Configs}/yuki.toml", Toml.FromModel(yukiConfig));

        app
            // Adapters
            // .UseKonataAdapter(konataConfig)
            .UseOneBotAdapter(GetOneBotAdapterConfig())

            // Plugins
            .Use(new StatusPlugin())
            .Use(new ArcaeaPlugin(yukiConfig.Plugins.Arcaea))
            .Use(new ImagesPlugin())
            .Use(new DebugPlugin())
            // .UseHttpCatPlugin(yukiConfig.Plugins.HttpCat)

            // Middlewares
            .Use(Middlewares.QqGuildFilter)

            // Start
            .Start();
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

    public static KonataAdapterConfig GetKonataAdapterConfig()
    {
        KonataAdapterConfig config;
        if (!File.Exists($"{YukiDir.Configs}/konata.toml"))
        {
            string? qq = null, password = null;
            while (qq is null || password is null)
            {
                Console.Write("QQ: ");
                qq = Console.ReadLine()?.Trim();
                Console.Write("Password: ");
                password = Console.ReadLine()?.Trim();
            }

            config = new KonataAdapterConfig(new List<KonataBotConfig>
            {
                new() { KeyStore = new BotKeyStore(qq, password) }
            });
            File.WriteAllText($"{YukiDir.Configs}/konata.toml", Toml.FromModel(config));
        }
        else
        {
            config = Toml.ToModel<KonataAdapterConfig>(File.ReadAllText($"{YukiDir.Configs}/konata.toml"));
        }

        return config;
    }

    #endregion
}