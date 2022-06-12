using System.Reflection;

// ReSharper disable ConvertToAutoProperty

namespace YukiChan.Utils;

public class BuildStamp
{
    public Type Type = null!;

    public string Branch => Stamp[0];

    public string CommitHash => Stamp[1][..16];

    public string Version => InformationalVersion;

    private string[] Stamp => Type
        .Assembly
        .GetCustomAttributes<AssemblyMetadataAttribute>()
        .First(x => x.Key is "BuildStamp")
        .Value!.Split(";");

    private string InformationalVersion => Type
        .Assembly
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
        .InformationalVersion;
}