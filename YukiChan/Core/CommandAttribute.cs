// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

using YukiChan.Database.Models;
using YukiChan.Utils;

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

public class Option
{
    public string ShortName { get; set; } = "";

    public string FullName { get; set; } = "";

    public OptionType Type { get; set; }

    public string Description { get; set; }

    public Option(string pattern, OptionType type, string description = "")
    {
        var ptns = pattern.Split(',');
        if (ptns.Length > 1)
        {
            ShortName = ptns[0].Trim().RemoveString("-");
            FullName = ptns[1].Trim().RemoveString("--");
        }
        else
        {
            var ptn = ptns[0].Trim();
            if (ptn.StartsWith("--"))
            {
                ShortName = "";
                FullName = ptn.RemoveString("--");
            }
            else if (ptn.StartsWith("-"))
            {
                ShortName = ptn.RemoveString("-");
                FullName = ShortName;
            }
        }

        Type = type;
        Description = description;
    }

    public Option(string pattern, string description = "")
        : this(pattern, OptionType.Boolean, description)
    {
    }
}

public enum OptionType
{
    String,
    Number,
    Boolean
}