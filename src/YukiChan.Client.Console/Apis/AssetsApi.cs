using YukiChan.Shared.Models.Arcaea;

namespace YukiChan.Client.Console.Apis;

public sealed class YukiConsoleAssetsApi : YukiConsoleBaseApi
{
    internal YukiConsoleAssetsApi(HttpClient client) : base(client)
    {
    }

    public Task<byte[]> GetArcaeaSongCover(string songId, ArcaeaDifficulty difficulty) =>
        Download($"v1/assets/arcaea/song/{songId}/{(int)difficulty}");

    public Task<byte[]> GetArcaeaCharImage(int charId, bool awakened) =>
        Download($"v1/assets/arcaea/char/{charId}?awakened={awakened}");

    public Task<byte[]> GetArcaeaPreviewImage(string songId, ArcaeaDifficulty difficulty) =>
        Download($"v1/assets/arcaea/preview/{songId}/{(int)difficulty}");

    public Task<byte[]> GetArcaeaSongDb() =>
        Download("v1/assets/arcaea/songDb");
}