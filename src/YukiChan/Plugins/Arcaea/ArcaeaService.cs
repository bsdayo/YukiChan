using YukiChan.ImageGen.Arcaea;
using YukiChan.Shared.Utils;

namespace YukiChan.Plugins.Arcaea;

public sealed class ArcaeaService
{
    public ArcaeaSongDbContext SongDb { get; }

    public ArcaeaImageGenerator ImageGenerator { get; }

    public ArcaeaService(ArcaeaSongDbContext songDb)
    {
        SongDb = songDb;
        ImageGenerator = new ArcaeaImageGenerator();
    }
}