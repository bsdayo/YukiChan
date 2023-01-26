using Flandre.Framework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YukiChan.Plugins.Arcaea;
using YukiChan.Plugins.SandBox;
using YukiChan.Shared.Utils;

namespace YukiChan.Plugins;

public static class PluginExtensions
{
    public static FlandreAppBuilder AddArcaeaPlugin(this FlandreAppBuilder builder, IConfiguration configuration)
    {
        builder.Services.AddSingleton<ArcaeaService>();
        builder.AddPlugin<ArcaeaPlugin, ArcaeaPluginOptions>(configuration);
        builder.Services.AddDbContext<ArcaeaSongDbContext>();
        return builder;
    }

    public static FlandreAppBuilder AddSandBoxPlugin(this FlandreAppBuilder builder, IConfiguration configuration)
    {
        builder.Services.AddSingleton<SandBoxService>();
        builder.AddPlugin<SandBoxPlugin, SandBoxPluginOptions>(configuration);
        return builder;
    }

    public static FlandreAppBuilder AddAutoAcceptPlugin(this FlandreAppBuilder builder, IConfiguration configuration)
    {
        return builder.AddPlugin<AutoAcceptPlugin, AutoAcceptPluginOptions>(configuration);
    }

    public static FlandreAppBuilder AddGosenPlugin(this FlandreAppBuilder builder, IConfiguration configuration)
    {
        return builder.AddPlugin<GosenPlugin, GosenPluginOptions>(configuration);
    }
}