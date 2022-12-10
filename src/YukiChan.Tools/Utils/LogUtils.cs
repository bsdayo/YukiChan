using Spectre.Console;

namespace YukiChan.Tools.Utils;

public static class LogUtils
{
    public static void Info(string message)
    {
        AnsiConsole.MarkupLine($"[blue bold]Info[/]: {message}");
    }

    public static void Error(string message)
    {
        AnsiConsole.MarkupLine($"[red bold]Error[/]: {message}");
    }

    public static void PathNotExists(string? path = null)
    {
        if (path is null)
            Error("The path does not exists. Please verify your input.");
        else
            Error($"The path [yellow]{path}[/] does not exists. Please verify your input.");
    }
}