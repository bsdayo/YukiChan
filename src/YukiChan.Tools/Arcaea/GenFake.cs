using Spectre.Console.Cli;
using YukiChan.ImageGen.Arcaea;
using YukiChan.Shared.Arcaea;
using YukiChan.Shared.Arcaea.Factories;
using YukiChan.Shared.Database.Models.Arcaea;
using YukiChan.Tools.Utils;

namespace YukiChan.Tools.Arcaea;

public sealed class GenFakeCommand : AsyncCommand<GenFakeCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<ImageType>")]
        public string ImageType { get; init; } = string.Empty;

        [CommandOption("-n|--nya")]
        public bool Nya { get; init; } = false;

        [CommandOption("-d|--dark")]
        public bool Dark { get; init; } = false;
    }

    public override async Task<int> ExecuteAsync(CommandContext ctx, Settings settings)
    {
        var generator = new ArcaeaImageGenerator();
        var pref = new ArcaeaUserPreferences
        {
            Nya = settings.Nya,
            Dark = settings.Dark
        };

        byte[] image;

        switch (settings.ImageType.ToLower())
        {
            case "b30":
            case "best30":
                image = await generator.Best30(ArcaeaBest30Factory.GenerateFake(), pref);
                break;

            case "single":
                image = await generator.SingleV1(
                    ArcaeaUserFactory.GenerateFake(),
                    ArcaeaRecordFactory.GenerateFake(
                        (await ArcaeaSongDatabase.Default.GetChartsById("equilibrium"))[2]),
                    null, pref);
                break;

            default:
                LogUtils.Error($"Image type {settings.ImageType} is not supported.");
                return 1;
        }

        await File.WriteAllBytesAsync($"fake-{settings.ImageType}.jpg", image);

        LogUtils.Info($"Fake {settings.ImageType} image saved to {
            Path.Combine(Directory.GetCurrentDirectory(), $"fake-{settings.ImageType}.jpg")}.");

        return 0;
    }
}