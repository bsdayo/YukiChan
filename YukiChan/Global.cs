using YukiChan.Database;

namespace YukiChan;

public static class Global
{
    public static YukiConfig YukiConfig { get; set; } = new();

    public static YukiDbManager YukiDb { get; set; } = new();
}