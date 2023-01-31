using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using YukiChan.Shared.Utils;
using YukiChan.Tools.Arcaea;
using YukiChan.Tools.Utils;

var services = new ServiceCollection();

services.AddDbContext<ArcaeaSongDbContext>();
services.AddScoped<ArcaeaFakeData>();

var app = new CommandApp(new TypeRegistrar(services));

app.Configure(config =>
{
    config.AddBranch("arcaea", arcaea =>
    {
        // arcaea.AddCommand<UpdateSongDbCommand>("update-songdb");
        arcaea.AddCommand<GenFakeCommand>("gen-fake");
    });
});

return app.Run(args);