using Flandre.Framework;
using Microsoft.Extensions.DependencyInjection;
using YukiChan.Plugins.Arcaea;

namespace YukiChan.Plugins;

public static class PluginExtensions
{
    public static FlandreAppBuilder UseArcaeaPlugin(this FlandreAppBuilder builder, ArcaeaPluginConfig? config = null)
    {
        builder.Services.AddSingleton<ArcaeaService>();
        builder.UsePlugin<ArcaeaPlugin, ArcaeaPluginConfig>(config ?? new ArcaeaPluginConfig());
        return builder;
    }
}