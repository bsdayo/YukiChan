using System.Reflection;
using Flandre.Framework;
using Flandre.Framework.Events;
using Microsoft.Extensions.DependencyInjection;
using YukiChan.Shared.Common;
using YukiChan.Shared.Database;
using YukiChan.Shared.Database.Models;

namespace YukiChan;

public static class YukiEventHandlers
{
    public static void OnCommandInvoking(FlandreApp app, CommandInvokingEvent e)
    {
        var authorityAttr = e.Command.InnerMethod.GetCustomAttribute<YukiUserAuthorityAttribute>();
        var db = app.Services.GetRequiredService<YukiDbManager>();
        var userData = db.GetUserDataOrDefault(e.Message.Platform, e.UserId)
            .GetAwaiter().GetResult();

        if (userData is null)
        {
            userData = new UserData
            {
                Platform = e.Message.Platform,
                UserId = e.UserId,
                UserAuthority = YukiUserAuthority.User
            };
            db.SaveUserData(userData).GetAwaiter().GetResult();
        }

        if (authorityAttr is not null &&
            userData.UserAuthority < authorityAttr.RequiredAuthority)
            e.Cancel();

        var dt = e.Message.Time.ToLocalTime();
        db.SaveCommandHistory(new CommandHistory
        {
            CallTime = dt,
            RespondTime = dt,
            Platform = e.Message.Platform,
            GuildId = e.GuildId ?? string.Empty,
            ChannelId = e.ChannelId ?? string.Empty,
            UserId = e.UserId,
            UserAuthority = userData.UserAuthority,
            Command = e.Command.CommandInfo.Command,
            CommandText = e.Message.GetText(),
            Response = null,
            IsSucceeded = false
        }).GetAwaiter().GetResult();
    }

    public static void OnCommandInvoked(FlandreApp app, CommandInvokedEvent e)
    {
        var db = app.Services.GetRequiredService<YukiDbManager>();
        db.UpdateCommandHistory(e.Message, e.Response?.GetText(), e.IsSucceeded).GetAwaiter().GetResult();
    }
}