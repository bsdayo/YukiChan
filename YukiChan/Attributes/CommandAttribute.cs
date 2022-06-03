using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace YukiChan.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class CommandAttribute : Attribute
{
    public string Name { get; }
    public string? Command { get; set; } = null;
    public string Description { get; set; } = "";
    public string Example { get; set; } = "";
    public bool Disabled { get; set; } = false;
    public SendType SendType { get; set; } = SendType.Send;

    public CommandAttribute(string name)
    {
        Name = name;
    }
}

public enum SendType
{
    Send,
    Reply,
}
