using System.Text.Json.Serialization;

#pragma warning disable CS8618

namespace YukiChan.Modules.Maimai.Models;

public class MaimaiChart
{
    [JsonPropertyName("achievements")] public double Achievements { get; set; }

    [JsonPropertyName("ds")] public double Const { get; set; }

    [JsonPropertyName("dxScore")] public int DxScore { get; set; }

    [JsonPropertyName("fc")] public string FullCombo { get; set; }

    [JsonPropertyName("fs")] public string FullSync { get; set; }

    [JsonPropertyName("level")] public string Level { get; set; }

    [JsonPropertyName("level_index")] public int LevelIndex { get; set; }

    [JsonPropertyName("level_label")] public string LevelLabel { get; set; }

    [JsonPropertyName("ra")] public int Rating { get; set; }

    [JsonPropertyName("rate")] public string Grade { get; set; }

    [JsonPropertyName("song_id")] public int SongId { get; set; }

    [JsonPropertyName("title")] public string Title { get; set; }

    [JsonPropertyName("type")] public string Type { get; set; }
}