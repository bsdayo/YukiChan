using System.IO;
using System.Text.Json;

namespace YukiChan.Core;

public class YukiConfig
{
    public string CommandPrefix { get; init; } = "/";
    public bool EnableDebugLog { get; init; } = true;

    public static YukiConfig GetYukiConfig()
    {
        if (File.Exists("Configs/YukiConfig.json"))
        {
            return JsonSerializer.Deserialize<YukiConfig>
                (File.ReadAllText("Configs/YukiConfig.json")) ?? new YukiConfig();
        }

        var config = new YukiConfig();
        var configJson = JsonSerializer.Serialize(config,
            new JsonSerializerOptions() { WriteIndented = true });
        File.WriteAllText("Configs/YukiConfig.json", configJson);

        return config;
    }
}