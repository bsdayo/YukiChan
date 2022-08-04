// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

using YukiChan.Database.Models;

namespace YukiChan.Core;

[AttributeUsage(AttributeTargets.Method)]
public sealed class CommandAttribute : Attribute
{
    // Command Meta
    public string Name { get; }
    public string? Description { get; set; } = null;
    public YukiUserAuthority Authority { get; set; } = YukiUserAuthority.User;
    public string? Usage { get; set; } = null;
    public string? Example { get; set; } = null;
    public bool Disabled { get; set; } = false;
    public bool Hidden { get; set; } = false;

    // Matching Rules
    public string? Command { get; set; } = null;
    public string? Shortcut { get; set; } = null;
    public string? StartsWith { get; set; } = null;
    public string? Regex { get; set; } = null;
    public string? Contains { get; set; } = null;
    public bool FallbackCommand { get; set; } = false;

    public CommandAttribute(string name)
    {
        Name = name;
    }
}

public enum SendType
{
    Send,
    Reply
}