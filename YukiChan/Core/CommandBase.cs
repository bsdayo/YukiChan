using System.Reflection;
using Konata.Core.Message;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace YukiChan.Core;

public class CommandBase
{
    public string ModuleCommand { get; }
    public CommandAttribute CommandInfo { get; }
    public MethodInfo InnerMethod { get; }

    public CommandBase(string moduleCommand, CommandAttribute commandInfo, MethodInfo innerMethod)
    {
        ModuleCommand = moduleCommand;
        CommandInfo = commandInfo;
        InnerMethod = innerMethod;
    }

    public MessageBuilder GetHelp()
    {
        var mb = new MessageBuilder()
            .Text("[帮助 - 指令]\n")
            .Text(
                $"{Global.YukiConfig.CommandPrefix}{CommandInfo.Usage ?? (CommandInfo.Command is null ? ModuleCommand : ModuleCommand + " " + CommandInfo.Command)}\n")
            .Text($"{CommandInfo.Description}");

        if (CommandInfo.Example is not null)
            return mb
                .Text("\n\n示例：\n")
                .Text(CommandInfo.Example);

        return mb;
    }
}