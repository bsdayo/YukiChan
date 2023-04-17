using Flandre.Adapters.OneBot;
using Flandre.Core.Common;
using Flandre.Framework;
using Flandre.Framework.Extensions;
using Flandre.Plugins.HttpCat;
using Flandre.Plugins.WolframAlpha;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Tomlyn.Extensions.Configuration;
using YukiChan.Client.Console;
using YukiChan.Core;

namespace YukiChan;

internal static class StartupExtensions
{
    internal static FlandreAppBuilder ConfigureInfrastructure(this FlandreAppBuilder builder, string[] args)
    {
        void AddYukiConfig(string name) =>
            builder.Configuration
                .AddTomlFile($"{YukiDir.Configs}/{name}.toml", true, true)
                .AddTomlFile($"{YukiDir.Configs}/{name}.{builder.Environment.EnvironmentName.ToLower()}.toml",
                    true, true);

        // 清除所有配置源
        builder.Configuration.Sources.Clear();
        // 添加 TOML 配置文件
        AddYukiConfig("common");
        AddYukiConfig("serilog");
        AddYukiConfig("plugins");
        AddYukiConfig("onebot");
        builder.Configuration
            // 添加环境变量配置
            .AddEnvironmentVariables("YUKICHAN_")
            // 添加命令行配置
            .AddCommandLine(args);

        builder.Services.ConfigureFlandreApp(builder.Configuration.GetSection("App"));
        builder.Services.Configure<YukiOptions>(builder.Configuration.GetSection("Yuki"));

        builder.Services.AddSingleton<YukiConsoleClient>();
        builder.Services.Configure<YukiConsoleClientOptions>(builder.Configuration.GetSection("Client"));

        return builder;
    }

    /// <summary>
    /// 配置 Serilog
    /// </summary>
    internal static FlandreAppBuilder ConfigureSerilog(this FlandreAppBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
        builder.Logging.ClearProviders().AddSerilog(dispose: true);
        return builder;
    }

    internal static FlandreAppBuilder CustomizePluginOptions(this FlandreAppBuilder builder)
    {
        builder.Services.PostConfigure<WolframAlphaPluginOptions>(cfg =>
            cfg.FontPath = $"{YukiDir.Assets}/fonts/TitilliumWeb-SemiBold.ttf");
        builder.Services.PostConfigure<HttpCatPluginOptions>(cfg =>
            cfg.CachePath = YukiDir.HttpCatCache);
        return builder;
    }

    internal static FlandreApp UpdateArcaeaSongDb(this FlandreApp app)
    {
        try
        {
            var client = app.Services.GetRequiredService<YukiConsoleClient>();
            var songDb = client.Assets.GetArcaeaSongDb().GetAwaiter().GetResult();
            File.WriteAllBytes($"{YukiDir.ArcaeaAssets}/arcsong.db", songDb);
        }
        catch (Exception e)
        {
            app.Services.GetRequiredService<ILogger<Program>>()
                .LogError(e, "自动更新 arcsong.db 失败");
        }

        return app;
    }
}