using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;
using YukiChan.Shared.Data;

namespace YukiChan.Client.Console.Apis;

public abstract class YukiConsoleBaseApi
{
    private readonly HttpClient _client;

    internal YukiConsoleBaseApi(HttpClient client) => _client = client;

    private static async Task<YukiResponse<TResponse>> ResolveData<TResponse>(HttpResponseMessage message)
        where TResponse : class
    {
        return (await message.Content.ReadFromJsonAsync<YukiResponse<TResponse>>(YukiResponse.JsonSerializerOptions))!;
    }

    private static async Task<YukiResponse> ResolveData(HttpResponseMessage message)
    {
        return (await message.Content.ReadFromJsonAsync<YukiResponse>(YukiResponse.JsonSerializerOptions))!;
    }

    protected async Task<YukiResponse<TResponse>> Get<TResponse>([StringSyntax("Uri")] string endpoint)
        where TResponse : class
    {
        using var message = await _client.GetAsync(endpoint);
        return await ResolveData<TResponse>(message);
    }

    protected async Task<YukiResponse<TResponse>> Post<TRequest, TResponse>([StringSyntax("Uri")] string endpoint,
        TRequest body)
        where TResponse : class
    {
        using var message = await _client.PostAsJsonAsync(endpoint, body, YukiResponse.JsonSerializerOptions);
        return await ResolveData<TResponse>(message);
    }

    protected async Task<YukiResponse<TResponse>> Put<TRequest, TResponse>([StringSyntax("Uri")] string endpoint,
        TRequest body)
        where TResponse : class
    {
        using var message = await _client.PutAsJsonAsync(endpoint, body, YukiResponse.JsonSerializerOptions);
        return await ResolveData<TResponse>(message);
    }

    protected async Task<YukiResponse> Put<TRequest>([StringSyntax("Uri")] string endpoint, TRequest body)
    {
        using var message = await _client.PutAsJsonAsync(endpoint, body, YukiResponse.JsonSerializerOptions);
        return await ResolveData(message);
    }

    protected async Task<byte[]> Download([StringSyntax("Uri")] string endpoint)
    {
        using var message = await _client.GetAsync(endpoint);

        if (message.StatusCode == HttpStatusCode.OK)
            return await message.Content.ReadAsByteArrayAsync();

        var resp = (await message.Content.ReadFromJsonAsync<YukiResponse>(YukiResponse.JsonSerializerOptions))!;
        throw new YukiApiException(message.StatusCode, resp.Code, resp.Message!);
    }
}