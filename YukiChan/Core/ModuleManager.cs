using System.Reflection;
using Konata.Core;
using Konata.Core.Message;
using YukiChan.Utils;

namespace YukiChan.Core;

public class MessageSession
{
    public long Timestamp { get; init; }
    public uint GroupUin { get; init; }
    public uint UserUin { get; init; }
#pragma warning disable CS8618
    public object Callback { get; init; }
#pragma warning restore CS8618
}

public static class ModuleManager
{
    public static Bot Bot { get; set; } = null!;

    public static readonly List<ModuleBase> Modules = new();

    public static int ModuleCount => Modules.Count;
    public static int CommandCount;

    public static readonly Dictionary<string, MessageSession> MessageSessions = new();

    public static void InitializeModules()
    {
        var types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (var type in types)
            // BotLogger.Debug($"Processing type {type.FullName}");
            // BotLogger.Debug($"  Base Class: {type.BaseType}");
            if (type.IsClass && type.BaseType == typeof(ModuleBase))
            {
                YukiLogger.Debug($"正在加载模块 {type.Name}");

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
                    YukiLogger.Error($"模块 {type.Name} 加载失败。\n"
                                     + e.Message + "\n" + e.StackTrace);
                }
            }

        // BotLogger.Debug("");
        Modules.Sort((moduleA, moduleB) =>
            string.Compare(moduleA.ModuleInfo.Name, moduleB.ModuleInfo.Name, StringComparison.CurrentCulture));

        YukiLogger.Success($"成功加载 {ModuleCount} 个模块，{CommandCount} 个指令。");
    }

    public static MessageBuilder? ParseCommand(Bot bot, MessageStruct message)
    {
        var sess = MessageSessions.Values.FirstOrDefault(
            s => s.GroupUin == message.Receiver.Uin && s.UserUin == message.Sender.Uin);
        if (sess is not null)
        {
            var identifier = $"{sess.GroupUin}-{sess.UserUin}";
            MessageSessions.Remove(identifier, out var session);
            YukiLogger.Debug($"Session {identifier} has been killed.");
            if (session!.Callback is Func<MessageStruct, MessageBuilder?> cbSync)
                return cbSync(message);
            if (session!.Callback is Func<MessageStruct, Task<MessageBuilder?>> cbAsync)
                return cbAsync(message).Result;
            return null;
        }

        foreach (var module in Modules)
        {
            var msgBuilder = module.DealCommand(bot, message);
            if (msgBuilder is null) continue;
            Global.Information.MessageProcessed++;
            YukiLogger.Debug($"Total {Global.Information.MessageProcessed} message(s) processed");
            return msgBuilder;
        }

        return null;
    }

    public static void GetSession(this Bot bot, MessageStruct message, int waitSeconds,
        Func<MessageStruct, MessageBuilder?> callback)
    {
        var groupUin = message.Receiver.Uin;
        var userUin = message.Sender.Uin;
        var identifier = $"{groupUin}-{userUin}";
        var timestamp = DateTime.Now.GetTimestamp();
        if (MessageSessions.ContainsKey(identifier)) return;

        MessageSessions.Add(identifier, new MessageSession
        {
            Timestamp = timestamp,
            GroupUin = groupUin,
            UserUin = userUin,
            Callback = callback
        });
        YukiLogger.Debug($"New message session {identifier} at {timestamp}.");

        Task.Run(async () =>
        {
            await Task.Delay(new TimeSpan(0, 0, 0, waitSeconds));
            if (MessageSessions.ContainsKey(identifier) && MessageSessions[identifier].Timestamp == timestamp)
            {
                MessageSessions.Remove(identifier);
                YukiLogger.Debug($"Message session {identifier} expired.");
                await bot.SendReply(message, "会话已超时。");
            }
        });
    }

    public static void GetSession(this Bot bot, MessageStruct message, int waitSeconds,
        Func<MessageStruct, Task<MessageBuilder?>> callback)
    {
        var groupUin = message.Receiver.Uin;
        var userUin = message.Sender.Uin;
        var identifier = $"{groupUin}-{userUin}";
        var timestamp = DateTime.Now.GetTimestamp();
        if (MessageSessions.ContainsKey(identifier)) return;

        MessageSessions.Add(identifier, new MessageSession
        {
            Timestamp = timestamp,
            GroupUin = groupUin,
            UserUin = userUin,
            Callback = callback
        });
        YukiLogger.Debug($"New message session {identifier} at {timestamp}.");

        Task.Run(async () =>
        {
            await Task.Delay(new TimeSpan(0, 0, 0, waitSeconds));
            if (MessageSessions.ContainsKey(identifier) && MessageSessions[identifier].Timestamp == timestamp)
            {
                MessageSessions.Remove(identifier);
                YukiLogger.Debug($"Message session {identifier} expired.");
                await bot.SendReply(message, "会话已超时。");
            }
        });
    }

    public static MessageBuilder GetHelp()
    {
        var mb = new MessageBuilder($"[帮助]\n当前已加载 {ModuleCount} 个模块，{CommandCount} 个指令\n");

        foreach (var module in Modules)
        {
            if (module.ModuleInfo.Hidden) continue;
            mb.Text(
                $"\n{Global.YukiConfig.CommandPrefix}{module.ModuleInfo.Command} {module.ModuleInfo.Description}");
        }

        return mb
            .Text($"\n\n请输入 {Global.YukiConfig.CommandPrefix}help <模块名> 查看详细信息。");
    }
}