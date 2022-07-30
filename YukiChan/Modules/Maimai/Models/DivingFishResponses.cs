using System.Text.Json.Serialization;

#pragma warning disable CS8618

namespace YukiChan.Modules.Maimai.Models;

public class DivingFishB40Response
{
    [JsonPropertyName("additional_rating")]
    public int AdditionalRating { get; set; }

    [JsonPropertyName("charts")] public DivingFishB40Charts Charts { get; set; }

    [JsonPropertyName("nickname")] public string Nickname { get; set; }

    [JsonPropertyName("plate")] public string Plate { get; set; }

    [JsonPropertyName("rating")] public int Rating { get; set; }

    [JsonPropertyName("user_data")] public object? UserData { get; set; }

    [JsonPropertyName("user_id")] public object? UserId { get; set; }

    [JsonPropertyName("username")] public string Username { get; set; }

    [JsonPropertyName("message")] public string? ErrorMessage { get; set; }

    public class DivingFishB40Charts
    {
        [JsonPropertyName("dx")] public MaimaiChart[] Dx { get; set; }

        [JsonPropertyName("sd")] public MaimaiChart[] Standard { get; set; }
    }
}