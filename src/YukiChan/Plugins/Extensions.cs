using Flandre.Framework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YukiChan.Plugins.Arcaea;
using YukiChan.Plugins.SandBox;
using YukiChan.Shared.Utils;

namespace YukiChan.Plugins;

public static class PluginExtensions
{
    public static IPluginCollection AddArcaea(this IPluginCollection plugins, IConfiguration configuration)
    {
        plugins.Services.AddSingleton<ArcaeaService>();
        plugins.Services.AddDbContext<ArcaeaSongDbContext>();
        plugins.Add<ArcaeaPlugin, ArcaeaPluginOptions>(configuration);
        return plugins;
    }

    public static IPluginCollection AddSandBox(this IPluginCollection plugins, IConfiguration configuration)
    {
        plugins.Services.AddSingleton<SandBoxService>();
        plugins.Add<SandBoxPlugin, SandBoxPluginOptions>(configuration);
        return plugins;
    }

    public static IPluginCollection AddAutoAccept(this IPluginCollection plugins, IConfiguration configuration)
    {
        return plugins.Add<AutoAcceptPlugin, AutoAcceptPluginOptions>(configuration);
    }

    public static IPluginCollection AddGosen(this IPluginCollection plugins, IConfiguration configuration)
    {
        return plugins.Add<GosenPlugin, GosenPluginOptions>(configuration);
    }
}