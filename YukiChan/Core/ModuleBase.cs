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
            BotLogger.Debug($"  检查方法 {method.Name}");

            if (method.ReturnType == typeof(void)) continue;

            var attrs = method.GetCustomAttributes(typeof(CommandAttribute), false);

            foreach (var attr in attrs)
            {
                BotLogger.Debug($"    检查特性 {attr.GetType()}");

                if (attr.GetType() != typeof(CommandAttribute)) continue;
                if (attr is not CommandAttribute command) continue;

                if (command.Disabled) continue;

                CommandBase commandBase = new(ModuleInfo.Command, command, method);
                Commands.Add(commandBase);
                BotLogger.Debug($"      加载指令 {t}.{method.Name} => {command.Name}");
            }
        }

        return CommandCount;
    }

    public MessageBuilder? DealCommand(Bot bot, MessageStruct message)
    {
        var commandStr = message.Chain.GetChain<TextChain>().Content.Trim();

        foreach (var command in Commands)
        {
            var keyword = Global.YukiConfig.CommandPrefix +
                          ModuleInfo.Command +
                          (command.CommandInfo.Command is not null
                              ? $" {command.CommandInfo.Command}"
                              : "");

            // 以标准指令格式开头
            var startsWithFlag = commandStr.StartsWith(keyword);
            // 匹配字符串包含
            var customStartFlag = command.CommandInfo.StartsWith is not null &&
                               commandStr.StartsWith(command.CommandInfo.StartsWith);
            // 匹配正则表达式
            var regexMatchFlag = command.CommandInfo.Regex is not null &&
                                 Regex.IsMatch(commandStr, command.CommandInfo.Regex);
            // 匹配字符串包含
            var containsFlag = command.CommandInfo.Contains is not null &&
                               commandStr.Contains(command.CommandInfo.Contains);

            if (!startsWithFlag && !regexMatchFlag && !customStartFlag && !containsFlag)
                continue;

            var user = Global.YukiDb.GetUser(message.Sender.Uin);

            if (user is null)
            {
                Global.YukiDb.AddUser(new YukiUser
                {
                    Uin = message.Sender.Uin,
                    Authority = YukiUserAuthority.User
                });

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

            var body = startsWithFlag
                ? commandStr[keyword.Length..].Trim()
                : commandStr;

            return Task.Run(() =>
            {
                try
                {
                    BotLogger.Debug($"Invoking command {command.CommandInfo.Name} with body \"{body}\".");
                    var result = command.InnerMethod.Invoke(this,
                        new object?[] { bot, message, body }[..command.InnerMethod.GetParameters().Length]);

                    return result as MessageBuilder ?? (result as Task<MessageBuilder>)?.Result ?? null;
                }
                catch (Exception exception)
                {
                    BotLogger.Error(exception);
                    return null;
                }
            }).Result;
        }

        return null;
    }

    public MessageBuilder GetHelp()
    {
        var helpStr = "[帮助 - 模块]\n";
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
                helpStr += "\n\nSubcommands:";
                isSubcommandTitleAdded = true;
            }

            helpStr += $"\n{command.CommandInfo.Command} {command.CommandInfo.Description}";
        }

        return MessageBuilder.Eval(helpStr);
    }
}