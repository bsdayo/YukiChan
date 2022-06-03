using System.IO;
using System.Text.Json;

namespace YukiChan.Core;

public class YukiConfig
{
    public string CommandPrefix { get; init; } = "/";
    public bool EnableDebugLog { get; init; } = true;

    public static YukiConfig GetYukiConfig()
    {
        if (File.Exists("yuki.config.json"))
        {
            return JsonSerializer.Deserialize<YukiConfig>
                (File.ReadAllText("yuki.config.json")) ?? new YukiConfig();
        }

        var config = new YukiConfig();
        var configJson = JsonSerializer.Serialize(config,
            new JsonSerializerOptions() { WriteIndented = true });
        File.WriteAllText("yuki.config.json", configJson);

        return config;
    }
}