namespace YukiChan.Plugins.Arcaea;

public class ArcaeaPluginOptions
{
    public string AuaApiUrl { get; init; } = string.Empty;
    public string AuaToken { get; init; } = string.Empty;
    public int AuaTimeout { get; init; } = 60;

    public string AlaToken { get; init; } = string.Empty;
    public int AlaTimeout { get; init; } = 60;

    public bool EnableGuess { get; init; } = true;
}