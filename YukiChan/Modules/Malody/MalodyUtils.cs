using MalodyInfoQuery.Model;
using YukiChan.Utils;

namespace YukiChan.Modules.Malody;

public class MalodyUtils
{
    public static async Task<byte[]> GetSongCover(MalodySongModel song)
    {
        byte[] songCover;

        try
        {
            var path = $"Cache/Malody/{song.SongName}.jpg";
            try
            {
                return await File.ReadAllBytesAsync(path);
            }
            catch
            {
                YukiLogger.Debug(song.CoverPicUrl);
                songCover = await NetUtils.DownloadBytes(song.CoverPicUrl);
                await File.WriteAllBytesAsync(path, songCover);
                YukiLogger.SaveCache(path);
            }
        }
        catch
        {
            songCover = await File.ReadAllBytesAsync("Assets/Arcaea/Images/SongCoverPlaceholder.jpg");
        }

        return songCover;
    }
}