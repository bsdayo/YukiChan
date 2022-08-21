using System.Text;
using Konata.Core;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces.Api;
using Konata.Core.Message;
using YukiChan.Utils;

namespace YukiChan.Core;

public static class EventHandlers
{
    public static async void OnBotOnline(Bot bot, BotOnlineEvent e)
    {
        var friendCount = (await bot.GetFriendList(true)).Count;
        var groupCount = (await bot.GetGroupList(true)).Count;
        YukiLogger.Success($"当前共有 {friendCount} 个好友，{groupCount} 个群聊。");
        YukiLogger.Success($"登录成功，{bot.Name} ({bot.Uin})。");
    }

    public static void OnBotOffline(Bot bot, BotOfflineEvent e)
    {
        YukiLogger.Warn("Bot 已离线，正在尝试重连...");
    }

    public static async void OnGroupMessage(Bot bot, GroupMessageEvent e)
    {
        if (Global.YukiDb.GetGroup(e.GroupUin) is null)
            Global.YukiDb.AddGroup(e.GroupUin);

        if (e.Message.Sender.Uin == bot.Uin) return;

        Global.Information.MessageReceived++;

        YukiLogger.ReceiveGroupMessage(e);

        try
        {
            var msgBuilder = ModuleManager.ParseCommand(bot, e.Message);
            await bot.SendGroupMessageWithLog(e.GroupName, e.GroupUin, msgBuilder);
        }
        catch (Exception exception)
        {
            YukiLogger.Error(exception);
            await bot.SendGroupMessageWithLog(e.GroupName, e.GroupUin,
                new MessageBuilder($"发生了奇怪的错误呢... ({exception.Message})"));
        }
    }

    public static async void OnFriendMessage(Bot bot, FriendMessageEvent e)
    {
        if (e.Message.Sender.Uin == bot.Uin) return;

        Global.Information.MessageReceived++;

        YukiLogger.ReceiveFriendMessage(e);

        try
        {
            var msgBuilder = ModuleManager.ParseCommand(bot, e.Message);
            await bot.SendFriendMessageWithLog(e.Message.Sender.Name, e.FriendUin, msgBuilder);
        }
        catch (Exception exception)
        {
            YukiLogger.Error(exception);
            await bot.SendFriendMessageWithLog(e.Message.Sender.Name, e.FriendUin,
                new MessageBuilder($"发生了奇怪的错误呢... ({exception.Message})"));
        }
    }

    public static async void OnGroupInvite(Bot bot, GroupInviteEvent e)
    {
        if (Global.YukiConfig.GroupInvitationProtect)
        {
            await bot.DeclineGroupInvitation(e.GroupUin, e.InviterUin, e.Token,
                "本 Bot 为限制范围使用，暂不通过群聊邀请，敬请谅解。");
            YukiLogger.Info($"由于开启群聊邀请保护，已拒绝加群 {e.GroupName} ({e.GroupUin})，邀请人为 {e.InviterNick} ({e.InviterUin})。");
            return;
        }

        if (Global.YukiConfig.GroupInvitationAdminRequired && !e.InviterIsAdmin) return;
        await bot.ApproveGroupInvitation(e.GroupUin, e.InviterUin, e.Token);
        YukiLogger.Info($"已加入群 {e.GroupName} ({e.GroupUin})，邀请人为 {e.InviterNick} ({e.InviterUin})。");
    }

    public static async void OnFriendRequest(Bot bot, FriendRequestEvent e)
    {
        if (Global.YukiConfig.FriendRequestProtect)
        {
            await bot.DeclineFriendRequest(e.ReqUin, e.Token);
            YukiLogger.Info(
                $"由于开启好友申请保护，已拒绝 {e.ReqNick} ({e.ReqUin}) 的好友申请，备注为 {e.ReqComment.ReplaceLineEndings("\\n")}。");
            return;
        }

        await bot.ApproveFriendRequest(e.ReqUin, e.Token);
        YukiLogger.Info($"已接受 {e.ReqNick} ({e.ReqUin}) 的好友申请，备注为 {e.ReqComment.ReplaceLineEndings("\\n")}。");
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
                YukiLogger.Warn("[Konata] " + e.EventMessage);
                break;
            case LogLevel.Exception:
            case LogLevel.Fatal:
                YukiLogger.Error("[Konata] " + e.EventMessage);
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