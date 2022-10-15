using Flandre.Adapters.Konata;
using Flandre.Adapters.OneBot;
using Flandre.Core;
using Konata.Core.Common;
using Tomlyn;
using YukiChan.Plugins;
using YukiChan.Utils;

namespace YukiChan;

public static class Program
{
    public static void Main()
    {
        YukiDir.EnsureExistence();

        var yukiConfig = GetYukiConfig();
        var konataConfig = GetKonataAdapterConfig();

        var app = new FlandreApp(yukiConfig.App);

        // Update Konata config
        app.OnAppReady += (_, _) =>
            File.WriteAllText($"{YukiDir.Configs}/konata.toml", Toml.FromModel(konataConfig));

        app
            // Adapters
            .UseKonataAdapter(konataConfig)
            .UseOneBotAdapter(GetOneBotAdapterConfig())

            // Plugins
            .Use(new StatusPlugin())

            // Start
            .Start();
    }

    public static YukiConfig GetYukiConfig()
    {
        YukiConfig config;
        if (!File.Exists($"{YukiDir.Configs}/yuki.toml"))
        {
            config = new YukiConfig();
            File.WriteAllText($"{YukiDir.Configs}/yuki.toml", Toml.FromModel(config, new TomlModelOptions()));
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
            File.WriteAllText($"{YukiDir.Configs}/onebot.toml", Toml.FromModel(config, new TomlModelOptions()));
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
}