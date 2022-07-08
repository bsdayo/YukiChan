using McMaster.Extensions.CommandLineUtils;
using System.IO.Compression;
using System.Text;
using SQLite;
using YukiChan.Modules.Arcaea.Models;

namespace YukiChan.Tools.Commands;

[Command(
    Name = "arcaea",
    FullName = "Arcaea",
    Description = "Arcaea Tools")]
[Subcommand(typeof(UnpackAssets), typeof(UpdateSongDb))]
public class ArcaeaCommand
{
    public void OnExecute(CommandLineApplication app)
    {
        app.ShowHelp();
    }


    [Command(
        Name = "unpack-assets",
        FullName = "Unpack Assets",
        Description = "Unpack Song Assets")]
    [HelpOption]
    public class UnpackAssets
    {
        [Argument(0, "APKFILE", "The .apk file of Arcaea")]
        public string? ApkFile { get; set; } = null;

        public void OnExecute(CommandLineApplication app)
        {
            if (string.IsNullOrWhiteSpace(ApkFile))
            {
                app.ShowHelp();
                return;
            }

            Console.WriteLine($"Unpacking {ApkFile}...");

            ZipFile.ExtractToDirectory(ApkFile, "Temp/Tools/ArcaeaUnpack/",
                Encoding.UTF8, true);

            Console.WriteLine("Copying files...");

            Directory.CreateDirectory("Cache/Arcaea/Song/");
            Directory.CreateDirectory("Assets/Arcaea/AudioPreview/");

            foreach (var dir in Directory.GetDirectories(
                         "Temp/Tools/ArcaeaUnpack/assets/songs/"))
            {
                var song = dir.Split('/')[^1];

                if (song is "pack" or "random" or "tutorial")
                    continue;

                var songId = song;
                var audioName = "base.ogg";
                var hasAffFile = true;

                if (song.StartsWith("dl_"))
                {
                    songId = song[3..];
                    audioName = "preview.ogg";
                    hasAffFile = false;
                }

                // Song Cover
                File.Copy($"Temp/Tools/ArcaeaUnpack/assets/songs/{song}/base.jpg",
                    $"Cache/Arcaea/Song/{songId}.jpg", true);

                if (File.Exists($"Temp/Tools/ArcaeaUnpack/assets/songs/{song}/3.jpg"))
                    File.Copy($"Temp/Tools/ArcaeaUnpack/assets/songs/{song}/3.jpg",
                        $"Cache/Arcaea/Song/{songId}-beyond.jpg", true);

                // Audio Preview
                if (audioName != "base.ogg")
                {
                    File.Copy($"Temp/Tools/ArcaeaUnpack/assets/songs/{song}/{audioName}",
                        $"Assets/Arcaea/AudioPreview/{songId}.ogg", true);

                    if (File.Exists($"Temp/Tools/ArcaeaUnpack/assets/songs/{song}/3_{audioName}"))
                        File.Copy($"Temp/Tools/ArcaeaUnpack/assets/songs/{song}/3_{audioName}",
                            $"Assets/Arcaea/AudioPreview/{songId}.ogg", true);
                }

                // Aff File
                if (hasAffFile)
                {
                    Directory.CreateDirectory($"Assets/Arcaea/Aff/{songId}/");

                    File.Copy($"Temp/Tools/ArcaeaUnpack/assets/songs/{song}/0.aff",
                        $"Assets/Arcaea/Aff/{songId}/Past.aff", true);
                    File.Copy($"Temp/Tools/ArcaeaUnpack/assets/songs/{song}/1.aff",
                        $"Assets/Arcaea/Aff/{songId}/Present.aff", true);
                    File.Copy($"Temp/Tools/ArcaeaUnpack/assets/songs/{song}/2.aff",
                        $"Assets/Arcaea/Aff/{songId}/Future.aff", true);
                }
            }

            Console.WriteLine("Song Cover => Cache/Arcaea/Song/");
            Console.WriteLine("Audio Preview => Assets/Arcaea/AudioPreview/");
            Console.WriteLine("Aff File => Assets/Arcaea/Aff/");

            Console.WriteLine("Removing temp folder...");
            Directory.Delete("Temp/Tools/ArcaeaUnpack/", true);

            Console.WriteLine("Done.");
        }
    }

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

            foreach (var chart in chartList)
                oldDb.InsertOrReplace(chart, typeof(ArcaeaSongDbChart));
            foreach (var alias in aliasList)
                oldDb.InsertOrReplace(alias, typeof(ArcaeaSongDbAlias));
            foreach (var package in packageList)
                oldDb.InsertOrReplace(package, typeof(ArcaeaSongDbPackage));

            Console.WriteLine(
                $"Total {chartList.Count} charts, {aliasList.Count} alias, {packageList.Count} packages.");
            Console.WriteLine("Done.");
        }
    }
}