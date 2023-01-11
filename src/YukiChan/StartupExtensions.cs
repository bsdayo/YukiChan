using Flandre.Adapters.OneBot;
using Flandre.Framework;
using Flandre.Framework.Extensions;
using Flandre.Plugins.HttpCat;
using Flandre.Plugins.WolframAlpha;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Tomlyn.Extensions.Configuration;
using YukiChan.Shared;
using YukiChan.Shared.Database;

namespace YukiChan;

internal static class StartupExtensions
{
    /// <summary>
    /// 添加配置源，更新 <see cref="FlandreAppOptions"/> 和 <see cref="YukiOptions"/>，注入 <see cref="YukiDbManager"/>
    /// </summary>
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

        builder.Services.AddSingleton<YukiDbManager>();

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

    internal static FlandreAppBuilder AddOneBotAdapter(this FlandreAppBuilder builder)
    {
        var config = builder.Configuration.GetSection("Adapters:OneBot").Get<OneBotAdapterConfig>();
        return config is not null ? builder.AddAdapter(new OneBotAdapter(config)) : builder;
    }

    internal static FlandreAppBuilder CustomizePluginOptions(this FlandreAppBuilder builder)
    {
        builder.Services.PostConfigure<WolframAlphaPluginOptions>(cfg =>
            cfg.FontPath = $"{YukiDir.Assets}/fonts/TitilliumWeb-SemiBold.ttf");
        builder.Services.PostConfigure<HttpCatPluginOptions>(cfg =>
            cfg.CachePath = YukiDir.HttpCatCache);
        return builder;
    }

    internal static FlandreApp LoadGuildAssignees(this FlandreApp app)
    {
        foreach (var guildData in app.Services.GetRequiredService<YukiDbManager>().GetAllGuildData().Result)
            app.SetGuildAssignee(guildData.Platform, guildData.GuildId, guildData.Assignee);
        return app;
    }
}