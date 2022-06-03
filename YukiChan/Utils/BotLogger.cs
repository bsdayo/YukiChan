using System;
using YukiChan.Core;

namespace YukiChan.Utils;

public static class BotLogger
{
    public static void Info(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("[INFO] ");
        Console.WriteLine(message);
    }
    
    public static void Warn(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("[WARN] ");
        Console.WriteLine(message);
    }
    
    public static void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("[ERROR] ");
        Console.WriteLine(message);
    }
    
    public static void Error(Exception exception)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("[ERROR] ");
        Console.WriteLine(exception.Message + "\n" + exception.StackTrace);
    }
    
    public static void Debug(string message)
    {
        if (!Global.YukiConfig.EnableDebugLog) return;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("[DEBUG] ");
        Console.WriteLine(message);
    }
}