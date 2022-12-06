using Spectre.Console.Cli;
using YukiChan.Tools.Arcaea;

var app = new CommandApp();

app.Configure(config =>
{
    config.AddBranch("arcaea", arcaea =>
    {
        arcaea.AddCommand<UpdateSongDbCommand>("update-songdb");
    });
});

return app.Run(args);