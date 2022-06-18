using File = System.IO.File;

namespace YukiChan.Utils;

public static class AssetsManager
{
    private static void LogAssetsSave(string path)
    {
        BotLogger.Info($"保存资源: {path}");
    }
    
    public static async Task<byte[]?> GetBytes(params string[] position)
    {
        var filename = string.Join('/', position);

        try
        {
            return await File.ReadAllBytesAsync($"Assets/{filename}");
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
            return await File.ReadAllTextAsync($"Assets/{filename}");
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
            Directory.CreateDirectory($"Assets/{directory}");
            await File.WriteAllBytesAsync($"Assets/{filename}", data);
            LogAssetsSave(filename);
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
            Directory.CreateDirectory($"Assets/{directory}");
            await File.WriteAllTextAsync($"Assets/{filename}", data);
            LogAssetsSave(filename);
            return true;
        }
        catch
        {
            return false;
        }
    }
}