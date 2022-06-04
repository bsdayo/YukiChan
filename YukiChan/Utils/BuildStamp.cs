using System.Linq;
using System.Reflection;
using Konata.Core;

// ReSharper disable ConvertToAutoProperty

namespace YukiChan.Utils;

public static class BuildStamp
{
    public static string Branch => Stamp[0];

    public static string CommitHash => Stamp[1][..16];

    public static string Version => InformationalVersion;
    
    private static readonly string[] Stamp = typeof(Bot)
        .Assembly
        .GetCustomAttributes<AssemblyMetadataAttribute>()
        .First(x => x.Key is "BuildStamp")
        .Value!.Split(";");

    private static readonly string InformationalVersion = typeof(Bot)
        .Assembly
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
        .InformationalVersion;
}