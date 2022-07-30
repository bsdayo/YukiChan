using System.Net.Http.Json;
using System.Text.Json;
using YukiChan.Modules.Maimai.Models;

namespace YukiChan.Modules.Maimai;

public class DivingFishClient
{
    private readonly HttpClient _client;

    public DivingFishClient(int timeout = 60)
    {
        _client = new HttpClient
        {
            Timeout = new TimeSpan(0, 0, 0, timeout)
        };
    }

    private async Task<DivingFishB40Response> GetB40(Dictionary<string, string> data)
    {
        var respMsg = await _client.PostAsync(
            "https://www.diving-fish.com/api/maimaidxprober/query/player",
            JsonContent.Create(data));
        var respText = await respMsg.Content.ReadAsStringAsync();
        var respData = JsonSerializer.Deserialize<DivingFishB40Response>(respText)!;

        if (respData.ErrorMessage is not null)
            throw new DivingFishApiException(respData.ErrorMessage);

        return respData;
    }

    public async Task<DivingFishB40Response> B40(string username)
    {
        var data = new Dictionary<string, string>
        {
            { "username", username }
        };
        return await GetB40(data);
    }

    public async Task<DivingFishB40Response> B40(uint qqUin)
    {
        var data = new Dictionary<string, string>
        {
            { "qq", qqUin.ToString() }
        };
        return await GetB40(data);
    }
}

public class DivingFishApiException : Exception
{
    public DivingFishApiException(string message) : base(message)
    {
    }
}