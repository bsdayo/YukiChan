using System.Text.RegularExpressions;
using Konata.Core;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using YukiChan.Database.Models;
using YukiChan.Utils;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace YukiChan.Core;

public abstract class ModuleBase
{
    internal Bot? Bot { get; set; }

    public readonly List<CommandBase> Commands = new();

    public ModuleAttribute ModuleInfo { get; set; } = null!;

    public int CommandCount => Commands.Count;

    public void Init()
    {
    }

    public void Reload()
    {
        Init();
    }

    public int LoadCommands()
    {
        var t = GetType();
        var methods = t.GetMethods();

        foreach (var method in methods)
        {
            YukiLogger.Debug($"  检查方法 {method.Name}");

            if (method.ReturnType == typeof(void)) continue;

            var attrs = method.GetCustomAttributes(typeof(CommandAttribute), false);

            foreach (var attr in attrs)
            {
                YukiLogger.Debug($"    检查特性 {attr.GetType()}");

                if (attr.GetType() != typeof(CommandAttribute)) continue;
                if (attr is not CommandAttribute command) continue;

                if (command.Disabled) continue;

                CommandBase commandBase = new(ModuleInfo.Command, command, method);
                Commands.Add(commandBase);
                YukiLogger.Debug($"      加载指令 {t}.{method.Name} => {command.Name}");
            }
        }

        Commands.Sort((commandA, commandB) =>
            string.Compare(commandA.CommandInfo.Command, commandB.CommandInfo.Command,
                StringComparison.CurrentCulture));

        return CommandCount;
    }

    public MessageBuilder? DealCommand(Bot bot, MessageStruct message)
    {
        var commandStr = message.Chain.GetChain<TextChain>()?.Content?.Trim();
        if (commandStr is null) return null;

        for (var i = 0; i < Commands.Count; i++)
        {
            var command = Commands[i];
            var primaryKeyword = Global.YukiConfig.CommandPrefix + ModuleInfo.Command;
            var keyword = primaryKeyword +
                          (command.CommandInfo.Command is not null
                              ? $" {command.CommandInfo.Command}"
                              : "");

            // 以标准指令格式开头
            var startsWithFlag = commandStr.StartsWith(keyword);
            // 匹配快捷短语开头
            var shortcutFlag = command.CommandInfo.Shortcut is not null &&
                               commandStr.StartsWith(command.CommandInfo.Shortcut.ToLower());
            // 匹配字符串包含
            var customStartFlag = command.CommandInfo.StartsWith is not null &&
                                  commandStr.StartsWith(command.CommandInfo.StartsWith);
            // 匹配正则表达式
            var regexMatchFlag = command.CommandInfo.Regex is not null &&
                                 Regex.IsMatch(commandStr, command.CommandInfo.Regex);
            // 匹配字符串包含
            var containsFlag = command.CommandInfo.Contains is not null &&
                               commandStr.Contains(command.CommandInfo.Contains);

            var fallbackFlag = command.CommandInfo.FallbackCommand && commandStr.Trim() == primaryKeyword;

            if (!startsWithFlag && !shortcutFlag && !regexMatchFlag && !customStartFlag && !containsFlag &&
                !fallbackFlag)
                continue;

            var group = Global.YukiDb.GetGroup(message.Receiver.Uin);
            if (group is null || group.Assignee != bot.Uin) return null;

            var callTime = DateTime.Now;
            var user = Global.YukiDb.GetUser(message.Sender.Uin);

            if (user is null)
            {
                Global.YukiDb.AddUser(message.Sender.Uin);

                if (command.CommandInfo.Authority > YukiUserAuthority.User)
                    return new MessageBuilder()
                        .Add(ReplyChain.Create(message))
                        .Text("权限不足哦~");
            }
            else
            {
                if (command.CommandInfo.Authority != YukiUserAuthority.Banned &&
                    user.Authority == YukiUserAuthority.Banned)
                    return null;

                if (user.Authority < command.CommandInfo.Authority)
                    return new MessageBuilder()
                        .Add(ReplyChain.Create(message))
                        .Text("权限不足哦~");
            }

            string body;

            if (startsWithFlag)
                body = commandStr[keyword.Length..].Trim();
            else if (shortcutFlag)
                body = commandStr[command.CommandInfo.Shortcut!.Length..].Trim();
            else if (fallbackFlag)
                body = "";
            else
                body = commandStr;

            return Task.Run(() =>
            {
                try
                {
                    YukiLogger.Debug($"Invoking command {command.CommandInfo.Name} with body \"{body}\".");
                    var result = command.InnerMethod.Invoke(this,
                        new object?[] { bot, message, body }[..command.InnerMethod.GetParameters().Length]);

                    var replyMb = result as MessageBuilder ?? (result as Task<MessageBuilder>)?.Result ?? null;

                    var replyTime = DateTime.Now;

                    Global.YukiDb.AddCommandHistory(callTime, replyTime, message.Type,
                        message.Receiver.Uin, message.Receiver.Name,
                        message.Sender.Uin, message.Sender.Name, user!.Authority,
                        ModuleInfo.Name, ModuleInfo.Command,
                        command.CommandInfo.Name, command.CommandInfo.Command ?? "",
                        commandStr, replyMb?.Build()?.ToString() ?? "(no reply)");

                    return replyMb;
                }
                catch (Exception exception)
                {
                    YukiLogger.Error(exception);
                    return null;
                }
            }).Result;
        }

        return null;
    }

    [Command("Help",
        Command = "help",
        Description = "获取模块帮助信息",
        Hidden = true)]
    public MessageBuilder GetHelp(bool addHeader = true)
    {
        var helpStr = addHeader ? "[帮助 - 模块]\n" : "";
        helpStr += $"{ModuleInfo.Name} {ModuleInfo.Version ?? "1.0.0"}\n";
        helpStr += $"{ModuleInfo.Description}";

        var isSubcommandTitleAdded = false;
        foreach (var command in Commands)
        {
            if (command.CommandInfo.Command is null)
            {
                helpStr += $"\n\n{Global.YukiConfig.CommandPrefix}{command.CommandInfo.Usage ?? ModuleInfo.Command}\n";
                helpStr += $"{command.CommandInfo.Description}";
                continue;
            }

            if (command.CommandInfo.Disabled || command.CommandInfo.Hidden)
                continue;

            if (!isSubcommandTitleAdded)
            {
                helpStr += "\n\nSubcommands:\n";
                isSubcommandTitleAdded = true;
            }

            helpStr += $"{command.CommandInfo.Command} {command.CommandInfo.Description}\n";
        }

        if (isSubcommandTitleAdded)
            helpStr += $"\n请输入 {Global.YukiConfig.CommandPrefix}help {ModuleInfo.Command} <子指令名> 查看详细信息。";

        return MessageBuilder.Eval(helpStr);
    }
}