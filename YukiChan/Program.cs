using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
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
        _bot = BotFather.Create(GetKonataConfig(), GetDevice(), GetKeyStore());

        ModuleManager.Bot = _bot;
        ModuleManager.InitializeModules();

        // Logger
        // _bot.OnLog += (_, e) => BotLogger.Info(e.EventMessage);

        // Captcha
        _bot.OnCaptcha += CaptchaUtil.OnCaptcha;

        _bot.OnGroupMessage += Response.OnGroupMessage;

        if (await _bot.Login()) UpdateKeyStore(_bot.KeyStore);
        
        BotLogger.Info($"Bot logined successfully as {_bot.Name} ({_bot.Uin}).");
    }

    private static BotConfig? GetKonataConfig()
    {
        if (File.Exists("konata.config.json"))
        {
            return JsonSerializer.Deserialize<BotConfig>
                (File.ReadAllText("konata.config.json"));
        }

        var config = BotConfig.Default();
        var configJson = JsonSerializer.Serialize(config,
            new JsonSerializerOptions() { WriteIndented = true });
        File.WriteAllText("konata.config.json", configJson);

        return config;
    }

    private static BotDevice? GetDevice()
    {
        if (File.Exists("device.json"))
        {
            return JsonSerializer.Deserialize<BotDevice>
                (File.ReadAllText("device.json"));
        }

        var device = BotDevice.Default();
        var deviceJson = JsonSerializer.Serialize(device,
            new JsonSerializerOptions() { WriteIndented = true });
        File.WriteAllText("device.json", deviceJson);

        return device;
    }

    private static BotKeyStore? GetKeyStore()
    {
        if (File.Exists("keystore.json"))
        {
            return JsonSerializer.Deserialize<BotKeyStore>
                (File.ReadAllText("keystore.json"));
        }

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
            new JsonSerializerOptions() { WriteIndented = true });
        File.WriteAllText("keystore.json", keystoreJson);
        return keystore;
    }
}