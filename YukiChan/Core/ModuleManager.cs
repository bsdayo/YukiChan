using System;
using System.Collections.Generic;
using System.Reflection;
using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Message;
using YukiChan.Attributes;
using YukiChan.Models;
using YukiChan.Utils;

namespace YukiChan.Core;

public static class ModuleManager
{
    public static Bot Bot { get; set; } = null!;

    public static readonly List<ModuleBase> Modules = new();
    
    public static void InitializeModules()
    {
        var types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (var type in types)
        {
            // BotLogger.Debug($"Processing type {type.FullName}");
            // BotLogger.Debug($"  Base Class: {type.BaseType}");
            
            if (type.IsClass && type.BaseType == typeof(ModuleBase))
            {
                BotLogger.Debug($"Loading module {type.Name}");

                if (Activator.CreateInstance(type) is not ModuleBase module)
                    continue;

                try
                {
                    var attr = type.GetCustomAttribute<ModuleAttribute>();
                    module.ModuleInfo = attr ?? new ModuleAttribute(nameof(module));
                    module.Bot = Bot;
                    module.LoadCommands();
                    module.Init();
                    Modules.Add(module);
                }
                catch (Exception e)
                {
                    BotLogger.Error($"Load module {type.Name} failed."
                                    + e.Message + "\n" + e.StackTrace);
                }
            }
            // BotLogger.Debug("");
        }
    }

    public static MessageBuilder? ParseCommand(Bot bot, GroupMessageEvent e)
    {
        foreach (var module in Modules)
        {
            if (module.DealCommand(bot, e) is { } mb)
            {
                Global.Information.MessageProcessed++;
                BotLogger.Debug($"Total {Global.Information.MessageProcessed} message(s) processed");
                return mb;
            }
        }

        return null;
    }
}