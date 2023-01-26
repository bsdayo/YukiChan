using YukiChan.Shared.Data;
using YukiChan.Shared.Data.Console.Arcaea;
using YukiChan.Shared.Models.Arcaea;

namespace YukiChan.Client.Console.Apis;

public sealed class YukiConsoleArcaeaApi : YukiConsoleBaseApi
{
    internal YukiConsoleArcaeaApi(HttpClient client) : base(client)
    {
    }


    #region 用户相关

    public Task<YukiResponse<ArcaeaBindResponse>> BindUser(string platform, string userId,
        ArcaeaBindRequest bindRequest) =>
        Put<ArcaeaBindRequest, ArcaeaBindResponse>($"v1/arcaea/users/{platform}/{userId}", bindRequest);

    public Task<YukiResponse<ArcaeaUserResponse>> GetUser(string platform, string userId) =>
        Get<ArcaeaUserResponse>($"v1/arcaea/users/{platform}/{userId}");

    #endregion

    #region 偏好相关

    public Task<YukiResponse> UpdatePreferences(string platform, string userId, ArcaeaPreferencesRequest pref) =>
        Put($"v1/arcaea/preferences/{platform}/{userId}", pref);

    public Task<YukiResponse<ArcaeaPreferencesResponse>> GetPreferences(string platform, string userId) =>
        Get<ArcaeaPreferencesResponse>($"v1/arcaea/preferences/{platform}/{userId}");

    #endregion

    #region 查分相关

    public Task<YukiResponse<ArcaeaBest30Response>> GetBest30(string target, bool official) =>
        Get<ArcaeaBest30Response>($"v1/arcaea/best30/{target}?official={official}");

    public Task<YukiResponse<ArcaeaBestResponse>> GetBest(string target, string song, ArcaeaDifficulty difficulty) =>
        Get<ArcaeaBestResponse>($"v1/arcaea/best/{target}/{song}/{(int)difficulty}");

    public Task<YukiResponse<ArcaeaRecentResponse>> GetRecent(string target) =>
        Get<ArcaeaRecentResponse>($"v1/arcaea/recent/{target}");

    #endregion

    #region 信息相关

    public Task<YukiResponse<ArcaeaSongIdResponse>> QuerySongId(string query) =>
        Get<ArcaeaSongIdResponse>($"v1/arcaea/songs/{query}/id");

    public Task<YukiResponse<ArcaeaSongAliasesResponse>> QuerySongAliases(string query) =>
        Get<ArcaeaSongAliasesResponse>($"v1/arcaea/songs/{query}/aliases");

    public Task<YukiResponse> AddSongAlias(string query, ArcaeaSongAddAliasRequest req) =>
        Put($"v1/arcaea/songs/{query}/aliases", req);

    #endregion
}