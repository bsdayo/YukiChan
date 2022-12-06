using System.Reflection;

namespace YukiChan.Utils;

public class BuildStamp
{
    private readonly Type _type;

    private string[] Stamp => _type
        .Assembly
        .GetCustomAttributes<AssemblyMetadataAttribute>()
        .First(x => x.Key is "BuildStamp")
        .Value!.Split(";");

    private string InformationalVersion => _type
        .Assembly
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
        .InformationalVersion;

    public string Branch => Stamp[0];

    public string CommitHash => Stamp[1][..16];

    public string Version => InformationalVersion;

    public BuildStamp(Type type)
    {
        _type = type;
    }
}