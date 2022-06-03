using System.Reflection;
using YukiChan.Attributes;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace YukiChan.Models;

public class CommandBase
{
    public CommandAttribute CommandInfo { get; }
    public MethodInfo InnerMethod { get; }

    public CommandBase(CommandAttribute commandInfo, MethodInfo innerMethod)
    {
        CommandInfo = commandInfo;
        InnerMethod = innerMethod;
    }
}