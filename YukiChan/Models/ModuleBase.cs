using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using YukiChan.Attributes;
using YukiChan.Core;
using YukiChan.Utils;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace YukiChan.Models;

public abstract class ModuleBase
{
    internal Bot? Bot { get; set; }

    private readonly List<CommandBase> _commands = new();

    public ModuleAttribute ModuleInfo { get; set; } = null!;

    public int CommandCount => _commands.Count;

    public void Init()
    {
    }

    public void Reload() => Init();

    public void LoadCommands()
    {
        var t = GetType();
        var methods = t.GetMethods();

        foreach (var method in methods)
        {
            BotLogger.Debug($"  Processing method {method.Name}");

            if (method.ReturnType == typeof(void)) continue;

            var attrs = method.GetCustomAttributes(typeof(CommandAttribute), false);

            foreach (var attr in attrs)
            {
                BotLogger.Debug($"    Processing attribute {attr.GetType()}");
                
                if (attr.GetType() != typeof(CommandAttribute)) continue;
                if (attr is not CommandAttribute command) continue;

                if (!command.Disabled)
                {
                    CommandBase commandBase = new(command, method);
                    _commands.Add(commandBase);
                    BotLogger.Debug($"      Loading command: {t}.{method.Name} => {command.Name}");
                }
            }
        }

        BotLogger.Info($"{CommandCount} commands loaded.");
    }

    public MessageBuilder? DealCommand(Bot bot, GroupMessageEvent e)
    {
        var commandStr = e.Chain.GetChain<TextChain>().Content.Trim();

        foreach (var command in _commands)
        {
            var keyword = Global.YukiConfig.CommandPrefix +
                          ModuleInfo.Command +
                          (command.CommandInfo.Command is not null
                              ? $" {command.CommandInfo.Command}"
                              : "");

            if (commandStr.StartsWith(keyword))
            {
                var body = commandStr[keyword.Length..].Trim();

                return Task.Run(() =>
                {
                    try
                    {
                        BotLogger.Debug($"Invoking command {command.CommandInfo.Name} with body \"{body}\".");
                        var result = command.InnerMethod.Invoke(this, new object?[] { bot, e, body });
                        return result as MessageBuilder ?? (result as Task<MessageBuilder>)?.Result;
                    }
                    catch (Exception exception)
                    {
                        BotLogger.Error(exception);
                        return null;
                    }
                }).Result;
            }
        }

        return null;
    }

    public string GetName()
    {
        var attr = GetType().GetCustomAttribute<ModuleAttribute>();
        if (attr is not null) return attr.Name;
        return GetType().FullName ?? "Unknown Module";
    }

    public string GetVersion()
    {
        var attr = GetType().GetCustomAttribute<ModuleAttribute>();
        return attr is not null ? attr.Version : "0.0.0";
    }
}