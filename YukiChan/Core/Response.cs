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
        if (e.MemberUin == bot.Uin) return;

        Global.Information.MessageReceived++;
        
        BotLogger.Debug($"Received message from {e.MemberUin}, total {Global.Information.MessageReceived} message(s).");

        var textChain = e.Chain.GetChain<TextChain>();
        if (textChain is null) return;

        try
        {
            if (ModuleManager.ParseCommand(bot, e) is { } mb)
            {
                Global.Information.MessageSent++;
                await bot.SendGroupMessage(e.GroupUin, mb);
            }
        }
        catch (Exception exception)
        {
            BotLogger.Error(exception);
        }
    }
}