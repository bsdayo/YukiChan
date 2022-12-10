using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using YukiChan.Shared.Database;
using YukiChan.Tools.Utils;

namespace YukiChan.Tools.Arcaea;

public sealed class UpdateSongDbCommand : AsyncCommand<UpdateSongDbCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<NewDbPath>")]
        [Description("The path of new arcsong.db")]
        public string NewDbPath { get; init; } = string.Empty;

        [CommandArgument(1, "[OldDbPath]")]
        [Description("The path of old arcsong.db")]
        public string OldDbPath { get; init; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext ctx, Settings settings)
    {
        if (!File.Exists(settings.NewDbPath))
        {
            LogUtils.PathNotExists(settings.NewDbPath);
            return 1;
        }

        var newDb = new ArcaeaSongDatabase(settings.NewDbPath);
        var oldDb = string.IsNullOrWhiteSpace(settings.OldDbPath)
            ? ArcaeaSongDatabase.Default
            : new ArcaeaSongDatabase(settings.OldDbPath);

        LogUtils.Info($"Merging [yellow]{settings.OldDbPath}[/] from [yellow]{settings.NewDbPath}[/]...");

        await AnsiConsole.Progress()
            .StartAsync(async progress =>
            {
                var chartTask = progress.AddTask("Migrating charts");
                var packageTask = progress.AddTask("Migrating packages");
                var aliasTask = progress.AddTask("Migrating aliases");

                var charts = await newDb.GetAllCharts();
                chartTask.MaxValue = charts.Count;
                var packages = await newDb.GetAllPackages();
                packageTask.MaxValue = packages.Count;
                var aliases = await newDb.GetAllAliases();
                aliasTask.MaxValue = aliases.Count;

                foreach (var chart in charts)
                {
                    await oldDb.InsertOrUpdateChart(chart);
                    chartTask.Increment(1);
                }

                foreach (var package in packages)
                {
                    await oldDb.InsertOrUpdatePackage(package);
                    packageTask.Increment(1);
                }

                foreach (var alias in aliases)
                {
                    await oldDb.InsertOrUpdateAlias(alias);
                    aliasTask.Increment(1);
                }
            });

        LogUtils.Info("Done.");

        return 0;
    }
}