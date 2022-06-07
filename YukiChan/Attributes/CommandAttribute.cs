using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace YukiChan.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class CommandAttribute : Attribute
{
    public string Name { get; }
    public string? Command { get; set; } = null;
    public string? Regex { get; set; } = null;
    public string? Contains { get; set; } = null;
    public string? Description { get; set; } = null;
    public string? Usage { get; set; } = null;
    public string? Example { get; set; } = null;
    public bool? Disabled { get; set; } = null;

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
