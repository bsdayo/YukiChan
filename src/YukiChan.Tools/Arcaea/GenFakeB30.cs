using Spectre.Console.Cli;
using YukiChan.ImageGen.Arcaea;
using YukiChan.Shared.Arcaea.Factories;
using YukiChan.Shared.Database.Models.Arcaea;
using YukiChan.Tools.Utils;

namespace YukiChan.Tools.Arcaea;

public sealed class GenFakeB30Command : AsyncCommand<GenFakeB30Command.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandOption("-n|--nya")] public bool Nya { get; init; } = false;

        [CommandOption("-d|--dark")] public bool Dark { get; init; } = false;
    }

    public override async Task<int> ExecuteAsync(CommandContext ctx, Settings settings)
    {
        var generator = new ArcaeaImageGenerator();

        var image = await generator.Best30(
            ArcaeaBest30Factory.GenerateFake(),
            new ArcaeaUserPreferences
            {
                Nya = settings.Nya,
                Dark = settings.Dark
            });

        await File.WriteAllBytesAsync("fake-b30.jpg", image);

        LogUtils.Info($"Fake Best30 image saved to {
            Path.Combine(Directory.GetCurrentDirectory(), "fake-b30.jpg")}.");

        return 0;
    }
}