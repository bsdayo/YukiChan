using System.Text;
using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces.Api;
using Konata.Core.Message;
using YukiChan.Core;

namespace YukiChan.Utils;

public static class YukiLogger
{
    private static bool _onLog;

    private static async void Log(string message, bool writeToFile)
    {
        while (_onLog) await Task.Delay(10);

        _onLog = true;
        var date = DateTime.Now.ToString("yyyy-MM-dd");
        var logMessage = DateTime.Now.ToString("HH:mm:ss ") + message;
        Console.WriteLine(logMessage);
        if (writeToFile)
        {
            var logFs = new FileStream($"Logs/YukiChan/{date}.log", FileMode.Append, FileAccess.Write,
                FileShare.ReadWrite);
            using (var sw = new StreamWriter(logFs, Encoding.UTF8))
            {
                sw.WriteLine(logMessage);
                sw.Flush();
            }
        }

        _onLog = false;
    }

    public static void Info(string message, bool writeToFile = true)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Log($"[I] {message}", writeToFile);
    }

    public static void Success(string message, bool writeToFile = true)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Log($"[S] {message}", writeToFile);
    }

    public static void Warn(string message, bool writeToFile = true)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Log($"[W] {message}", writeToFile);
    }

    public static void Error(string message, bool writeToFile = true)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Log($"[E] {message}", writeToFile);
    }

    public static void Error(Exception exception, bool writeToFile = true)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Log($"[E] {exception.GetType().Name}: {exception.Message}\n{exception.StackTrace}", writeToFile);
    }

    public static void Debug(string message, bool writeToFile = true)
    {
        if (!Global.YukiConfig.EnableDebugLog) return;
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Log($"[D] {message}", writeToFile);
    }

    public static async void ReceiveGroupMessage(GroupMessageEvent e)
    {
        while (_onLog) await Task.Delay(10);

        _onLog = true;
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write(DateTime.Now.ToString("HH:mm:ss ") +
                      $"[M] <R:G> {e.GroupName} ({e.GroupUin}) {e.Message.Sender.Name} ({e.Message.Sender.Uin}) => ");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(e.Message.Chain.ToString().ReplaceLineEndings("\\n"));
        _onLog = false;
    }

    public static async void ReceiveFriendMessage(FriendMessageEvent e)
    {
        while (_onLog) await Task.Delay(10);

        _onLog = true;
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write(DateTime.Now.ToString("HH:mm:ss ") +
                      $"[M] <R:F> {e.Message.Sender.Name} ({e.FriendUin}) => ");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(e.Message.Chain.ToString().ReplaceLineEndings("\\n"));
        _onLog = false;
    }

    public static async Task SendGroupMessageWithLog(this Bot bot, string groupName, uint groupUin, MessageBuilder? mb)
    {
        if (mb is null) return;

        await bot.SendGroupMessage(groupUin, mb);

        while (_onLog) await Task.Delay(10);

        _onLog = true;
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write(DateTime.Now.ToString("HH:mm:ss ") +
                      $"[M] <S:G> {groupName} ({groupUin}) => ");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(mb.Build().ToString().ReplaceLineEndings("\\n"));
        _onLog = false;

        Global.Information.MessageSent++;
    }

    public static async Task SendFriendMessageWithLog(this Bot bot, string friendName, uint friendUin,
        MessageBuilder? mb)
    {
        if (mb is null) return;

        await bot.SendFriendMessage(friendUin, mb);

        while (_onLog) await Task.Delay(10);

        _onLog = true;
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write(DateTime.Now.ToString("HH:mm:ss ") +
                      $"[M] <S:F> {friendName} ({friendUin}) => ");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(mb.Build().ToString().ReplaceLineEndings("\\n"));
        _onLog = false;
    }

    public static void SaveCache(string path)
    {
        Info($"[Cache] 保存缓存: {path}");
    }
}

public class ModuleLogger
{
    private string ModuleName { get; }

    public ModuleLogger(string moduleName)
    {
        ModuleName = moduleName;
    }

    public void Info(string message, bool writeToFile = true)
    {
        YukiLogger.Info($"[{ModuleName}] {message}", writeToFile);
    }

    public void Success(string message, bool writeToFile = true)
    {
        YukiLogger.Success($"[{ModuleName}] {message}", writeToFile);
    }

    public void Warn(string message, bool writeToFile = true)
    {
        YukiLogger.Warn($"[{ModuleName}] {message}", writeToFile);
    }

    public void Error(string message, bool writeToFile = true)
    {
        YukiLogger.Error($"[{ModuleName}] {message}", writeToFile);
    }

    public void Error(Exception exception, bool writeToFile = true)
    {
        YukiLogger.Error($"[{ModuleName}] {exception.GetType().Name}: {exception.Message}\n{exception.StackTrace}",
            writeToFile);
    }

    public void Debug(string message, bool writeToFile = true)
    {
        YukiLogger.Debug($"[{ModuleName}] {message}", writeToFile);
    }
}