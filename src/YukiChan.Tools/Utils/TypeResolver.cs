using Spectre.Console.Cli;

namespace YukiChan.Tools.Utils;

public class TypeResolver : ITypeResolver
{
    private readonly IServiceProvider _provider;

    public TypeResolver(IServiceProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public object? Resolve(Type? type)
    {
        return type is null
            ? null
            : _provider.GetService(type);
    }
}