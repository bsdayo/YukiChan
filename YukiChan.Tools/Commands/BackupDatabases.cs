using System.IO.Compression;
using System.Text;
using McMaster.Extensions.CommandLineUtils;
using YukiChan.Utils;

namespace YukiChan.Tools.Commands;

[Command(
    Name = "backup-databases",
    FullName = "Backup Databases",
    Description = "Backup all YukiChan's Databases")]
[HelpOption]
public class BackupDatabases
{
    public void OnExecute(CommandLineApplication app)
    {
        if (!Directory.Exists("DatabaseBackups"))
            Directory.CreateDirectory("DatabaseBackups");
        if (!Directory.Exists("Temp/DatabasesCopy"))
            Directory.CreateDirectory("Temp/DatabasesCopy");

        foreach (var db in Directory.GetFiles("Databases"))
            File.Copy(db, $"Temp/DatabasesCopy{db.RemoveString("Databases")}", true);


        var today = DateTime.Today;
        var path = $"DatabaseBackups/{today.Year}-{today.Month}-{today.Day}.zip";

        if (File.Exists(path))
            File.Delete(path);

        ZipFile.CreateFromDirectory("Temp/DatabasesCopy", path,
            CompressionLevel.Optimal, false, Encoding.UTF8);

        Console.WriteLine($"Databases backuped to {path}.");
    }
}