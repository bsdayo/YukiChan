using System.Text.Json;

namespace YukiChan.Core;

public class YukiConfig
{
    public string MasterUin { get; init; } = "";
    public string CommandPrefix { get; init; } = "/";
    public bool EnableDebugLog { get; init; } = true;
    public bool GroupInvitationProtect { get; init; } = true;
    public bool GroupInvitationAdminRequired { get; init; }
    public bool FriendRequestProtect { get; init; } = true;

    public ArcaeaConfig Arcaea { get; init; } = new();

    public WolframAlphaConfig WolframAlpha { get; init; } = new();

    public BaiduTranslateConfig BaiduTranslate { get; init; } = new();

    public MalodyConfig Malody { get; init; } = new();

    public GosenConfig Gosen { get; init; } = new();

    public static YukiConfig GetYukiConfig()
    {
        var config = new YukiConfig();

        if (File.Exists("Configs/YukiConfig.json"))
            config = JsonSerializer.Deserialize<YukiConfig>
                (File.ReadAllText("Configs/YukiConfig.json"))!;

        var configJson = JsonSerializer.Serialize(config,
            new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("Configs/YukiConfig.json", configJson);

        return config;
    }
}

public class ArcaeaConfig
{
    public string AuaApiUrl { get; init; } = "";
    public string AuaToken { get; init; } = "";
    public int AuaTimeout { get; init; } = 60;

    public string AlaToken { get; init; } = "";
    public int AlaTimeout { get; init; } = 60;

    public bool EnableGuess { get; init; } = true;
}

public class WolframAlphaConfig
{
    public string AppId { get; init; } = "";
}

public class BaiduTranslateConfig
{
    public string AppId { get; init; } = "";
    public string Token { get; init; } = "";
}

public class MalodyConfig
{
    public string Account { get; init; } = "";
    public string Password { get; init; } = "";
    public string BaseUrl { get; init; } = "https://m.mugzone.net";
}

public class GosenConfig
{
    public string ApiUrl { get; init; } = "";
}