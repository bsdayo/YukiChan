using System.Text;
using Konata.Core;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces.Api;
using Konata.Core.Message.Model;
using YukiChan.Utils;

namespace YukiChan.Core;

public static class EventHandlers
{
    public static async void OnBotOnline(Bot bot, BotOnlineEvent e)
    {
        var friendCount = (await bot.GetFriendList(true)).Count;
        var groupCount = (await bot.GetGroupList(true)).Count;
        BotLogger.Success($"当前共有 {friendCount} 个好友，{groupCount} 个群聊。");
        BotLogger.Success($"登录成功，{bot.Name} ({bot.Uin})。");
    }

    public static void OnBotOffline(Bot bot, BotOfflineEvent e)
    {
        BotLogger.Warn("Bot 已离线，正在尝试重连...");
    }

    public static async void OnGroupMessage(Bot bot, GroupMessageEvent e)
    {
        if (e.Message.Sender.Uin == bot.Uin) return;

        Global.Information.MessageReceived++;

        BotLogger.ReceiveMessage(e);

        var textChain = e.Chain.GetChain<TextChain>();
        if (textChain is null) return;

        try
        {
            var msgBuilder = ModuleManager.ParseCommand(bot, e.Message);
            if (msgBuilder is null) return;
            Global.Information.MessageSent++;
            BotLogger.SendMessage(e, msgBuilder.Build().ToString());
            await bot.SendGroupMessage(e.GroupUin, msgBuilder);
        }
        catch (Exception exception)
        {
            BotLogger.Error(exception);
        }
    }

    public static async void OnFriendMessage(Bot bot, FriendMessageEvent e)
    {
        if (e.Message.Sender.Uin == bot.Uin) return;

        Global.Information.MessageReceived++;

        BotLogger.ReceiveMessage(e);

        var textChain = e.Chain.GetChain<TextChain>();
        if (textChain is null) return;

        try
        {
            var msgBuilder = ModuleManager.ParseCommand(bot, e.Message);
            if (msgBuilder is null) return;
            Global.Information.MessageSent++;
            BotLogger.SendMessage(e, msgBuilder.Build().ToString());
            await bot.SendFriendMessage(e.FriendUin, msgBuilder);
        }
        catch (Exception exception)
        {
            BotLogger.Error(exception);
        }
    }

    public static async void OnLog(Bot bot, LogEvent e)
    {
        var date = DateTime.Now.ToString("yyyy-MM-dd");
        var time = DateTime.Now.ToString("HH:mm:ss");
        var logMessage = time +
                         $" [{e.Level.ToString()[0]}]" +
                         $" <{e.Tag}> " +
                         e.EventMessage;
        switch (e.Level)
        {
            case LogLevel.Warning:
                BotLogger.Warn("[Konata] " + e.EventMessage);
                break;
            case LogLevel.Exception:
            case LogLevel.Fatal:
                BotLogger.Error("[Konata] " + e.EventMessage);
                break;
        }

        var logFs = new FileStream($"Logs/Konata/{date}.log", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        await using var sw = new StreamWriter(logFs, Encoding.UTF8);
        await sw.WriteLineAsync(logMessage);
        await sw.FlushAsync();
    }

    public static void OnCaptcha(Bot bot, CaptchaEvent e)
    {
        switch (e.Type)
        {
            case CaptchaEvent.CaptchaType.Sms:
                Console.WriteLine($"The verify code has been sent to {e.Phone}.");
                Console.Write("Code: ");
                bot.SubmitSmsCode(Console.ReadLine());
                break;

            case CaptchaEvent.CaptchaType.Slider:
                Console.WriteLine("Need slider captcha.");
                Console.Write($"Url: {e.SliderUrl}\nTicket: ");
                bot.SubmitSliderTicket(Console.ReadLine());
                break;

            case CaptchaEvent.CaptchaType.Unknown:
            default:
                break;
        }
    }
}