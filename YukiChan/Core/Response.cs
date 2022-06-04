using System;
using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces.Api;
using Konata.Core.Message.Model;
using YukiChan.Utils;

namespace YukiChan.Core;

public static class Response
{
    public static async void OnGroupMessage(Bot bot, GroupMessageEvent e)
    {
        if (e.Message.Sender.Uin == bot.Uin) return;

        Global.Information.MessageReceived++;

        BotLogger.ReceiveMessage(e);

        var textChain = e.Chain.GetChain<TextChain>();
        if (textChain is null) return;

        try
        {
            if (ModuleManager.ParseCommand(bot, e.Message) is { } mb)
            {
                Global.Information.MessageSent++;
                BotLogger.SendMessage(e, mb.Build().ToString());
                await bot.SendGroupMessage(e.GroupUin, mb);
            }
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
            if (ModuleManager.ParseCommand(bot, e.Message) is { } mb)
            {
                Global.Information.MessageSent++;
                BotLogger.SendMessage(e, mb.Build().ToString());
                await bot.SendFriendMessage(e.FriendUin, mb);
            }
        }
        catch (Exception exception)
        {
            BotLogger.Error(exception);
        }
    }
}