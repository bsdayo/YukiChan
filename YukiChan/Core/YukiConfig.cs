using System.Text.Json;

namespace YukiChan.Core;

public class YukiConfig
{
    public string CommandPrefix { get; init; } = "/";
    public bool EnableDebugLog { get; init; } = true;
    public bool GroupInvitationProtect { get; init; } = true;
    public bool GroupInvitationAdminRequired { get; init; } = false;
    public bool FriendRequestProtect { get; init; } = true;

    public YukiConfigArcaea Arcaea { get; init; } = new();

    public YukiConfigWolframAlpha WolframAlpha { get; init; } = new();

    public YukiConfigBaiduTranslate BaiduTranslate { get; init; } = new();

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

public class YukiConfigArcaea
{
    public string AuaApiUrl { get; init; } = "";
    public string UserAgent { get; init; } = "";
    public int Timeout { get; init; } = 60;
}

public class YukiConfigWolframAlpha
{
    public string AppId { get; init; } = "";
}

public class YukiConfigBaiduTranslate
{
    public string AppId { get; init; } = "";
    public string Token { get; init; } = "";
}