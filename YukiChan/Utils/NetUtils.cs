using System.Net;

namespace YukiChan.Utils;

public static class NetUtils
{
    public static async Task<byte[]> DownloadBytes(string url,
        Dictionary<string, string>? header = null,
        int timeout = 8000, long limitLen = ((long)2 << 30) - 1)
    {
        var request = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.All })
        {
            Timeout = new TimeSpan(0, 0, 0, timeout),
            MaxResponseContentBufferSize = limitLen
        };

        request.DefaultRequestHeaders.Add("User-Agent", new[]
        {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64)",
            "AppleWebKit/537.36 (KHTML, like Gecko)",
            "Chrome/100.0.4896.127 Safari/537.36 Edg/100.0.1185.50",
            "YukiChan/1.0.0"
        });

        if (header is not null)
            foreach (var (k, v) in header)
                request.DefaultRequestHeaders.Add(k, v);

        var response = await request.GetByteArrayAsync(url);

        return response;
    }
}