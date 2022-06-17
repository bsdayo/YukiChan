using System.Text;
using Konata.Core;
using Konata.Core.Message;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using YukiChan.Core;
using YukiChan.Database.Models;
using YukiChan.Utils;

namespace YukiChan.Modules;

[Module("Code",
    Command = "code",
    Description = "运行 C# 代码",
    Version = "1.0.0")]
public class CodeModule : ModuleBase
{
    private static ScriptState<object>? _userState;
    private static ScriptState<object>? _execState;

    private static readonly ScriptOptions UserOptions = ScriptOptions.Default
        .WithFileEncoding(Encoding.UTF8)
        .AddReferences("Microsoft.CSharp")
        .AddImports("System")
        .AddImports("System.Collections.Generic")
        .AddImports("System.Linq");

    private static readonly ScriptOptions ExecOptions = UserOptions
        .AddImports("System.IO")
        .AddImports("System.Reflection")
        .AddImports("System.Net");

    private static readonly string[] BannedNamespaces =
    {
        "System.IO",
        "System.Net",
        "Konata.Core",
        "SQLite",
        "Microsoft.CodeAnalysis",
        "System.Diagnostics",
        "System.Drawing",
        "System.Reflection",
        "System.Resources",
        "System.Runtime",
        "System.Windows",
        "WindowsBase"
    };

    private static readonly string[] BannedStatements =
    {
        "Console.",
        "Pow",
        "typeof"
    };

    [Command("Run Code",
        Command = "run",
        Description = "运行 C# 代码")]
    public static async Task<MessageBuilder?> RunCode(Bot bot, MessageStruct message, string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return CommonUtils.ReplyMessage(message)
                .Text("请输入需要运行的代码哦~");

        foreach (var ns in BannedNamespaces)
            if (body.Contains(ns))
                return CommonUtils.ReplyMessage(message)
                    .Text("不可以使用这些命名空间哦~");

        foreach (var st in BannedStatements)
            if (body.Contains(st))
                return CommonUtils.ReplyMessage(message)
                    .Text("不可以使用这些语句哦~");

        BotLogger.Info($"Running Code: {body}");

        try
        {
            _userState = _userState is null
                ? await CSharpScript.RunAsync(body, UserOptions)
                : await _userState.ContinueWithAsync(body, UserOptions);
        }
        catch (Exception e)
        {
            return CommonUtils.ReplyMessage(message)
                .Text(e.Message);
        }

        BotLogger.Info(
            $"...with return value: {_userState?.ReturnValue?.ToString()?.ReplaceLineEndings("\\n") ?? "null"}");

        if (_userState?.ReturnValue is not null)
            return CommonUtils.ReplyMessage(message)
                .Text(_userState.ReturnValue?.ToString());

        return null;
    }

    [Command("Execute Code (Admin Mode)",
        Command = "exec",
        Authority = YukiUserAuthority.Owner,
        Description = "运行 C# 代码",
        Hidden = true)]
    public static async Task<MessageBuilder?> ExecCode(Bot bot, MessageStruct message, string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return CommonUtils.ReplyMessage(message)
                .Text("请输入需要运行的代码哦~");

        BotLogger.Info($"Executing Code: {body}");

        try
        {
            _execState = _execState is null
                ? await CSharpScript.RunAsync(body, ExecOptions)
                : await _execState.ContinueWithAsync(body, ExecOptions);
        }
        catch (Exception e)
        {
            return CommonUtils.ReplyMessage(message)
                .Text(e.Message);
        }

        BotLogger.Info($"...with return value: {_execState?.ReturnValue?.ToString() ?? "null"}");

        if (_execState?.ReturnValue is not null)
            return CommonUtils.ReplyMessage(message)
                .Text(_execState.ReturnValue?.ToString());

        return null;
    }

    [Command("Reset state",
        Command = "reset",
        Authority = YukiUserAuthority.Admin,
        Description = "重置命名空间")]
    public static MessageBuilder Reset(Bot bot, MessageStruct message, string body)
    {
        _userState = null;
        if (body == "exec") _execState = null;
        return CommonUtils.ReplyMessage(message)
            .Text("成功重置命名空间。");
    }

    [Command("Variables",
        Command = "vars",
        Description = "查看全局变量")]
    public static MessageBuilder Vars(Bot bot, MessageStruct message, string body)
    {
        if (_userState is null)
            return CommonUtils.ReplyMessage(message)
                .Text("命名空间尚未初始化，请执行一次代码后重试。");

        var mb = CommonUtils.ReplyMessage(message)
            .Text($"共 {_userState.Variables.Length} 个变量");

        foreach (var variable in _userState.Variables) mb.Text($"\n{variable.Name}: {variable.Type}");

        return mb;
    }
}