using System;
using System.IO;
using System.Threading.Tasks;
using Konata.Core.Events.Model;
using YukiChan.Core;

namespace YukiChan.Utils;

public static class BotLogger
{
    private static bool _onLog;

    private static async void Log(string message)
    {
        while (_onLog)
        {
            await Task.Delay(10);
        }
        _onLog = true;
        var logMessage = DateTime.Now.ToString("HH:mm:ss ") + message;
        Console.WriteLine(logMessage);
        _onLog = false;
    }
    
    public static void Info(string message)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Log($"[I] {message}");
    }
    
    public static void Success(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Log($"[S] {message}");
    }
    
    public static void Warn(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Log($"[W] {message}");
    }
    
    public static void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Log($"[E] {message}");
    }
    
    public static void Error(Exception exception)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Log($"[E] {exception.Message}\n{exception.StackTrace}");
    }
    
    public static void Debug(string message)
    {
        if (!Global.YukiConfig.EnableDebugLog) return;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Log($"[D] {message}");
    }
    
    public static async void ReceiveMessage(GroupMessageEvent e)
    {
        while (_onLog)
        {
            await Task.Delay(10);
        }
        _onLog = true;
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write(DateTime.Now.ToString("HH:mm:ss ") +
                      $"[M] <R:G> {e.GroupName} ({e.GroupUin}) | {e.Message.Sender.Name} ({e.Message.Sender.Uin}) => ");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(e.Message.Chain.ToString().ReplaceLineEndings("\\n"));
        _onLog = false;
    }
    
    public static async void ReceiveMessage(FriendMessageEvent e)
    {
        while (_onLog)
        {
            await Task.Delay(10);
        }
        _onLog = true;
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write(DateTime.Now.ToString("HH:mm:ss ") +
                      $"[M] <R:F> {e.Message.Sender.Name} ({e.FriendUin}) => ");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(e.Message.Chain.ToString().ReplaceLineEndings("\\n"));
        _onLog = false;
    }
    
    public static async void SendMessage(GroupMessageEvent e, string? message)
    {
        while (_onLog)
        {
            await Task.Delay(10);
        }
        _onLog = true;
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write(DateTime.Now.ToString("HH:mm:ss ") +
                      $"[M] <S:G> {e.GroupName} ({e.GroupUin}) => ");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(message?.ReplaceLineEndings("\\n"));
        _onLog = false;
    }
    
    public static async void SendMessage(FriendMessageEvent e, string? message)
    {
        while (_onLog)
        {
            await Task.Delay(10);
        }
        _onLog = true;
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write(DateTime.Now.ToString("HH:mm:ss ") +
                      $"[M] <S:F> {e.Message.Receiver.Name} ({e.FriendUin}) => ");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(message?.ReplaceLineEndings("\\n"));
        _onLog = false;
    }
}