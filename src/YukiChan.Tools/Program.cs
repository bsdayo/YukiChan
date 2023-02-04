using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Tomlyn.Extensions.Configuration;
using YukiChan.Client.Console;
using YukiChan.Core;
using YukiChan.Shared.Utils;
using YukiChan.Tools.Arcaea;
using YukiChan.Tools.Utils;

var services = new ServiceCollection();
var configuration = new ConfigurationBuilder()
    .AddYukiConfig("common")
    .Build();

services.AddDbContext<ArcaeaSongDbContext>();
services.AddScoped<ArcaeaFakeData>();
services.AddSingleton<YukiConsoleClient>();
services.Configure<YukiConsoleClientOptions>(configuration.GetSection("Client"));

var app = new CommandApp(new TypeRegistrar(services));

app.Configure(config =>
{
    config.AddBranch("arcaea", arcaea =>
    {
        // arcaea.AddCommand<UpdateSongDbCommand>("update-songdb");
        arcaea.AddCommand<GenFakeCommand>("gen-fake");
        arcaea.AddCommand<HandleAliasSubmissionCommand>("handle-alias-submissions");
    });
});

return app.Run(args);

file static class Extensions
{
    internal static IConfigurationBuilder AddYukiConfig(this IConfigurationBuilder builder, string name) =>
        builder.AddTomlFile($"{YukiDir.Configs}/{name}.toml", true, true);
}