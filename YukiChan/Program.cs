using System.Text.Json;
using Konata.Core;
using Konata.Core.Common;
using Konata.Core.Interfaces;
using Konata.Core.Interfaces.Api;
using YukiChan.Core;
using YukiChan.Utils;

namespace YukiChan;

public static class Program
{
    private static Bot _bot = null!;

    public static async Task Main()
    {
        try
        {
            InitializeDirectories(new[]
            {
                // Logs - Yuki and Konata's log
                "Logs/YukiChan", "Logs/Konata",
                // Configs - Yuki and Konata's configs
                "Configs",
                // Assets - Module assets
                "Assets/Cats",
                "Assets/Arcaea/AudioPreview", "Assets/Arcaea/Aff",
                // Cache - Download cache
                "Cache/Arcaea/Song/"
            });

            _bot = BotFather.Create(GetKonataConfig(), GetDevice(), GetKeyStore());

            ModuleManager.Bot = _bot;
            ModuleManager.InitializeModules();

            // Konata log
            _bot.OnLog += EventHandlers.OnLog;

            // Captcha
            _bot.OnCaptcha += EventHandlers.OnCaptcha;

            _bot.OnBotOnline += EventHandlers.OnBotOnline;
            _bot.OnBotOffline += EventHandlers.OnBotOffline;

            _bot.OnGroupMessage += EventHandlers.OnGroupMessage;
            _bot.OnFriendMessage += EventHandlers.OnFriendMessage;

            if (await _bot.Login()) UpdateKeyStore(_bot.KeyStore);
        }
        catch (Exception e)
        {
            BotLogger.Error(e);
            Environment.Exit(1);
        }
    }

    private static BotConfig? GetKonataConfig()
    {
        if (File.Exists("Configs/KonataConfig.json"))
            return JsonSerializer.Deserialize<BotConfig>
                (File.ReadAllText("Configs/KonataConfig.json"));

        var config = BotConfig.Default();
        var configJson = JsonSerializer.Serialize(config,
            new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("Configs/KonataConfig.json", configJson);

        return config;
    }

    private static BotDevice? GetDevice()
    {
        if (File.Exists("Configs/Device.json"))
            return JsonSerializer.Deserialize<BotDevice>
                (File.ReadAllText("Configs/Device.json"));

        var device = BotDevice.Default();
        var deviceJson = JsonSerializer.Serialize(device,
            new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("Configs/Device.json", deviceJson);

        return device;
    }

    private static BotKeyStore? GetKeyStore()
    {
        if (File.Exists("Configs/KeyStore.json"))
            return JsonSerializer.Deserialize<BotKeyStore>
                (File.ReadAllText("Configs/KeyStore.json"));

        Console.WriteLine("For first running, please enter your account and password.");
        Console.Write("Account: ");
        var account = Console.ReadLine();
        Console.Write("Password: ");
        var password = Console.ReadLine();

        Console.WriteLine("Bot created.");
        return UpdateKeyStore(new BotKeyStore(account, password));
    }

    private static BotKeyStore UpdateKeyStore(BotKeyStore keystore)
    {
        var keystoreJson = JsonSerializer.Serialize(keystore,
            new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("Configs/KeyStore.json", keystoreJson);
        return keystore;
    }

    private static void InitializeDirectories(string[] directories)
    {
        foreach (var dir in directories)
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
    }
}