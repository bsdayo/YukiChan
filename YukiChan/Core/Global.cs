using YukiChan.Database;

namespace YukiChan.Core;

public static class Global
{
    public static readonly YukiConfig YukiConfig = YukiConfig.GetYukiConfig();

    public static readonly YukiDbManager YukiDb = new();

    public static class Information
    {
        public static int MessageProcessed = 0;
        public static int MessageReceived = 0;
        public static int MessageSent = 0;
    }
}