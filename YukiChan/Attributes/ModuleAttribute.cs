using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace YukiChan.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ModuleAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; set; } = "";
    public string Version { get; set; } = "1.0.0";
    public string Command { get; set; } = "";

    public ModuleAttribute(string name)
    {
        Name = name;
    }
}