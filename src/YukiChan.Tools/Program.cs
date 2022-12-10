using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using YukiChan.Tools.Arcaea;
using YukiChan.Tools.Utils;

var services = new ServiceCollection();

var app = new CommandApp(new TypeRegistrar(services));

app.Configure(config =>
{
    config.AddBranch("arcaea", arcaea => { arcaea.AddCommand<UpdateSongDbCommand>("update-songdb"); });
});

return app.Run(args);