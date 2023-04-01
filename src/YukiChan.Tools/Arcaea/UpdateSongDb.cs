using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Spectre.Console.Cli;
using YukiChan.Shared.Utils;
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

        await using var newDb = new ArcaeaSongDbContext(settings.NewDbPath);
        await using var oldDb = string.IsNullOrWhiteSpace(settings.OldDbPath)
            ? new ArcaeaSongDbContext()
            : new ArcaeaSongDbContext(settings.OldDbPath);

        LogUtils.Info($"Merging [yellow]{settings.OldDbPath}[/] from [yellow]{settings.NewDbPath}[/]...");
        var charts = await newDb.Charts.ToListAsync();
        var packages = await newDb.Packages.ToListAsync();
        var aliases = await newDb.Aliases.ToListAsync();

        foreach (var chart in charts)
            oldDb.Charts.Update(chart);
        foreach (var package in packages)
            oldDb.Packages.Update(package);
        foreach (var alias in aliases)
            oldDb.Aliases.Update(alias);

        LogUtils.Info("Saving changes...");
        await oldDb.SaveChangesAsync();

        LogUtils.Info("Done.");

        return 0;
    }
}