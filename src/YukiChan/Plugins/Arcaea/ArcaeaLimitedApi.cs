﻿using System.Text.Json;
using System.Text.Json.Serialization;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace YukiChan.Plugins.Arcaea;

public class AlaClient
{
    private readonly HttpClient _httpClient = new();

    public required string Token { get; set; }

    public string ApiUrl { get; set; } = "https://arcaea-limitedapi.lowiro.com/api/v0";

    public int Timeout { get; set; } = 60;

    public AlaClient()
    {
        _httpClient.BaseAddress = new Uri(ApiUrl);
        _httpClient.Timeout = new TimeSpan(0, 0, 0, Timeout);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
    }

    public async Task<AlaRecord[]> Best30(string usercode)
    {
        try
        {
            var json = await _httpClient.GetStringAsync($"user/{usercode}/best");
            var resp = JsonSerializer.Deserialize<AlaResponse<AlaRecord[]>>(json)!;
            if (resp.Message is not null)
                throw new AlaException(resp.Message);
            return resp.Data;
        }
        catch (Exception e)
        {
            throw new AlaException(e.Message);
        }
    }

    public async Task<AlaUser> User(string usercode)
    {
        try
        {
            var json = await _httpClient.GetStringAsync($"user/{usercode}");
            var resp = JsonSerializer.Deserialize<AlaResponse<AlaUser>>(json)!;
            if (resp.Message is not null)
                throw new AlaException(resp.Message);
            return resp.Data;
        }
        catch (Exception e)
        {
            throw new AlaException(e.Message);
        }
    }
}

#region Models

#pragma warning disable CS8618

public class AlaRecord
{
    [JsonPropertyName("song_id")] public string SongId { get; set; }

    [JsonPropertyName("difficulty")] public int Difficulty { get; set; }

    [JsonPropertyName("score")] public int Score { get; set; }

    [JsonPropertyName("shiny_pure_count")] public int ShinyPureCount { get; set; }

    [JsonPropertyName("pure_count")] public int PureCount { get; set; }

    [JsonPropertyName("far_count")] public int FarCount { get; set; }

    [JsonPropertyName("lost_count")] public int LostCount { get; set; }

    [JsonPropertyName("recollection_rate")]
    public int RecollectionRate { get; set; }

    [JsonPropertyName("time_played")] public long TimePlayed { get; set; }

    [JsonPropertyName("gauge_type")] public int GaugeType { get; set; }
}

public class AlaUser
{
    [JsonPropertyName("display_name")] public string DisplayName { get; set; }
    [JsonPropertyName("potential")] public double? Potential { get; set; }
    [JsonPropertyName("partner")] public PartnerInfo Partner { get; set; }
    [JsonPropertyName("last_played_song")] public AlaRecord LastPlayedSong { get; set; }

    public class PartnerInfo
    {
        [JsonPropertyName("partner_id")] public int PartnerId { get; set; }
        [JsonPropertyName("is_awakened")] public bool IsAwakened { get; set; }
    }
}

public class AlaResponse<T>
{
    [JsonPropertyName("data")] public T Data { get; set; }

    [JsonPropertyName("message")] public string? Message { get; set; }
}

public class AlaException : Exception
{
    public AlaException(string message) : base(message)
    {
    }
}

#endregion