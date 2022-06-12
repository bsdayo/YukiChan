using System.Reflection;
using Konata.Core;
using Konata.Core.Message;
using YukiChan.Attributes;
using YukiChan.Models;
using YukiChan.Utils;

namespace YukiChan.Core;

public static class ModuleManager
{
    public static Bot Bot { get; set; } = null!;

    public static readonly List<ModuleBase> Modules = new();

    public static int ModuleCount => Modules.Count;
    public static int CommandCount;
    
    public static void InitializeModules()
    {
        var types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (var type in types)
        {
            // BotLogger.Debug($"Processing type {type.FullName}");
            // BotLogger.Debug($"  Base Class: {type.BaseType}");
            
            if (type.IsClass && type.BaseType == typeof(ModuleBase))
            {
                BotLogger.Debug($"正在加载模块 {type.Name}");

                if (Activator.CreateInstance(type) is not ModuleBase module)
                    continue;

                try
                {
                    var attr = type.GetCustomAttribute<ModuleAttribute>();
                    module.ModuleInfo = attr ?? new ModuleAttribute(nameof(module));
                    module.Bot = Bot;
                    CommandCount += module.LoadCommands();
                    module.Init();
                    Modules.Add(module);
                }
                catch (Exception e)
                {
                    BotLogger.Error($"模块 {type.Name} 加载失败。\n"
                                    + e.Message + "\n" + e.StackTrace);
                }
            }
            // BotLogger.Debug("");
        }
        BotLogger.Success($"成功加载 {ModuleCount} 个模块，{CommandCount} 个指令。");
    }

    public static MessageBuilder? ParseCommand(Bot bot, MessageStruct message)
    {
        foreach (var module in Modules)
        {
            var msgBuilder = module.DealCommand(bot, message);
            if (msgBuilder is null) continue;
            Global.Information.MessageProcessed++;
            BotLogger.Debug($"Total {Global.Information.MessageProcessed} message(s) processed");
            return msgBuilder;
        }
        
        return null;
    }

    public static MessageBuilder GetHelp()
    {
        var helpStr = $"[帮助]\n当前已加载 {ModuleCount} 个模块，{CommandCount} 个指令\n";

        foreach (var module in Modules)
        {
            helpStr +=
                $"\n{Global.YukiConfig.CommandPrefix}{module.ModuleInfo.Command} {module.ModuleInfo.Description}";
        }
        
        return MessageBuilder.Eval(helpStr);
    }
}