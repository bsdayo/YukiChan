namespace YukiChan.Utils;

public static class CacheManager
{
    private static void LogCacheSave(string path)
    {
        YukiLogger.Info($"保存缓存: {path}");
    }

    public static async Task<byte[]?> GetBytes(string path)
    {
        try
        {
            return await File.ReadAllBytesAsync($"Cache/{path}");
        }
        catch
        {
            return null;
        }
    }

    public static async Task<string?> GetText(params string[] position)
    {
        var filename = string.Join('/', position);

        try
        {
            return await File.ReadAllTextAsync($"Cache/{filename}");
        }
        catch
        {
            return null;
        }
    }

    public static async Task SaveBytes(byte[] data, string path)
    {
        var directory = string.Join('/', path.Split("/")[..^1]);

        Directory.CreateDirectory($"Cache/{directory}");
        await File.WriteAllBytesAsync($"Cache/{path}", data);
        LogCacheSave(path);
    }

    public static async Task<bool> SaveText(string data, params string[] position)
    {
        var filename = string.Join('/', position);
        var directory = string.Join('/', position[..^1]);

        try
        {
            Directory.CreateDirectory($"Cache/{directory}");
            await File.WriteAllTextAsync($"Cache/{filename}", data);
            LogCacheSave(filename);
            return true;
        }
        catch
        {
            return false;
        }
    }
}