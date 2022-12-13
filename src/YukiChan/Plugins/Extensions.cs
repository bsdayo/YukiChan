using Flandre.Framework;
using Microsoft.Extensions.DependencyInjection;
using YukiChan.Plugins.Arcaea;
using YukiChan.Plugins.SandBox;

namespace YukiChan.Plugins;

public static class PluginExtensions
{
    public static FlandreAppBuilder UseArcaeaPlugin(this FlandreAppBuilder builder, ArcaeaPluginConfig? config = null)
    {
        builder.Services.AddSingleton<ArcaeaService>();
        builder.UsePlugin<ArcaeaPlugin, ArcaeaPluginConfig>(config ?? new ArcaeaPluginConfig());
        return builder;
    }

    public static FlandreAppBuilder UseSandBoxPlugin(this FlandreAppBuilder builder, SandBoxPluginConfig? config = null)
    {
        builder.Services.AddSingleton<SandBoxService>();
        builder.UsePlugin<SandBoxPlugin, SandBoxPluginConfig>(config ?? new SandBoxPluginConfig());
        return builder;
    }
}