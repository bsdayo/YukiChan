namespace YukiChan.Client.Console;

public sealed class YukiConsoleClientOptions
{
    public string ApiUrl { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public int Timeout { get; set; } = 20;
}