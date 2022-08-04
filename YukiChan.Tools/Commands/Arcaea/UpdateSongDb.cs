using McMaster.Extensions.CommandLineUtils;
using SQLite;
using YukiChan.Modules.Arcaea.Models;

namespace YukiChan.Tools.Commands.Arcaea;

[Command(
    Name = "update-songdb",
    FullName = "Update arcsong.db",
    Description = "Update arcsong.db")]
[HelpOption]
public class UpdateSongDb
{
    [Argument(0, "SONGDBFILE", "The new arcsong.db")]
    public string? NewDbFile { get; set; } = null;

    public void OnExecute(CommandLineApplication app)
    {
        if (string.IsNullOrWhiteSpace(NewDbFile))
        {
            app.ShowHelp();
            return;
        }

        if (!File.Exists("Assets/Arcaea/arcsong.db"))
        {
            Console.WriteLine("Original arcsong.db not exists. Auto copying.");
            File.Copy(NewDbFile, "Assets/Arcaea/arcsong.db");
            Console.WriteLine("Done.");
            return;
        }

        var oldDb = new SQLiteConnection("Assets/Arcaea/arcsong.db");
        var newDb = new SQLiteConnection(NewDbFile);

        oldDb.Execute("PRAGMA foreign_keys = OFF");

        var chartList = newDb.Query<ArcaeaSongDbChart>(
            "SELECT * FROM charts");
        var aliasList = newDb.Query<ArcaeaSongDbAlias>(
            "SELECT * FROM alias");
        var packageList = newDb.Query<ArcaeaSongDbPackage>(
            "SELECT * FROM packages");

        oldDb.RunInTransaction(() =>
        {
            foreach (var chart in chartList)
                oldDb.InsertOrReplace(chart, typeof(ArcaeaSongDbChart));
            foreach (var alias in aliasList)
                oldDb.InsertOrReplace(alias, typeof(ArcaeaSongDbAlias));
            foreach (var package in packageList)
                oldDb.InsertOrReplace(package, typeof(ArcaeaSongDbPackage));
        });

        Console.WriteLine(
            $"Total {chartList.Count} charts, {aliasList.Count} alias, {packageList.Count} packages.");
        Console.WriteLine("Done.");
    }
}