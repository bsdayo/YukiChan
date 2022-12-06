namespace YukiChan.Plugins.Arcaea;

public class ArcaeaPluginConfig
{
    public string AuaApiUrl { get; init; } = "";
    public string AuaToken { get; init; } = "";
    public int AuaTimeout { get; init; } = 60;

    public string AlaToken { get; init; } = "";
    public int AlaTimeout { get; init; } = 60;

    public bool EnableGuess { get; init; } = true;
}