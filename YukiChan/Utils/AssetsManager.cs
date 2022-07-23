namespace YukiChan.Utils;

public static class AssetsManager
{
    private static void LogAssetsSave(string path)
    {
        YukiLogger.Info($"保存资源: {path}");
    }

    public static async Task<byte[]?> GetBytes(string path)
    {
        try
        {
            return await File.ReadAllBytesAsync($"Assets/{path}");
        }
        catch
        {
            return null;
        }
    }

    public static async Task<string?> GetText(string path)
    {
        try
        {
            return await File.ReadAllTextAsync($"Assets/{path}");
        }
        catch
        {
            return null;
        }
    }

    public static async Task SaveBytes(byte[] data, string path)
    {
        var directory = string.Join('/', path.Split("/")[..^1]);

        Directory.CreateDirectory($"Assets/{directory}");
        await File.WriteAllBytesAsync($"Assets/{path}", data);
        LogAssetsSave(path);
    }

    public static async Task SaveText(string data, string path)
    {
        var directory = string.Join('/', path.Split("/")[..^1]);

        Directory.CreateDirectory($"Assets/{directory}");
        await File.WriteAllTextAsync($"Assets/{path}", data);
        LogAssetsSave(path);
    }
}