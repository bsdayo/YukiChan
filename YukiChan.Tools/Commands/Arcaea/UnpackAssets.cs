using System.IO.Compression;
using System.Text;
using McMaster.Extensions.CommandLineUtils;

namespace YukiChan.Tools.Commands.Arcaea;

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