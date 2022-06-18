using File = System.IO.File;

namespace YukiChan.Utils;

public static class CacheManager
{
    private static void LogCacheSave(string path)
    {
        BotLogger.Info($"保存缓存: {path}");
    }
    
    public static async Task<byte[]?> GetBytes(params string[] position)
    {
        var filename = string.Join('/', position);

        try
        {
            return await File.ReadAllBytesAsync($"Cache/{filename}");
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

    public static async Task<bool> SaveBytes(byte[] data, params string[] position)
    {
        var filename = string.Join('/', position);
        var directory = string.Join('/', position[..^1]);

        try
        {
            Directory.CreateDirectory($"Cache/{directory}");
            await File.WriteAllBytesAsync($"Cache/{filename}", data);
            LogCacheSave(filename);
            return true;
        }
        catch
        {
            return false;
        }
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