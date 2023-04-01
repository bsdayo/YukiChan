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

        LogUtils.Info($"Merging [yellow]{settings.NewDbPath}[/]...");
        var charts = await newDb.Charts.AsNoTracking().ToListAsync();
        var packages = await newDb.Packages.AsNoTracking().ToListAsync();
        var aliases = await newDb.Aliases.AsNoTracking().ToListAsync();

        LogUtils.Info("Updating charts...");
        foreach (var chart in charts)
        {
            if (await oldDb.Charts.AnyAsync(c => c.SongId == chart.SongId && c.RatingClass == chart.RatingClass))
                oldDb.Charts.Update(chart);
            else
                oldDb.Charts.Add(chart);
        }


        LogUtils.Info("Updating packages...");
        foreach (var package in packages)
        {
            if (await oldDb.Packages.AnyAsync(p => p.Set == package.Set))
                oldDb.Packages.Update(package);
            else
                oldDb.Packages.Add(package);
        }

        LogUtils.Info("Updating aliases...");
        foreach (var alias in aliases)
        {
            if (!await oldDb.Aliases.AnyAsync(a => a.SongId == alias.SongId && a.Alias == alias.Alias))
                oldDb.Aliases.Add(alias);
        }

        LogUtils.Info("Saving changes...");
        await oldDb.SaveChangesAsync();

        LogUtils.Info("Done.");

        return 0;
    }
}