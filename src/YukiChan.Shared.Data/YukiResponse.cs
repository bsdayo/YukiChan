using System.Text.Json;
using System.Text.Json.Serialization;

namespace YukiChan.Shared.Data;

public class YukiResponse
{
    public static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public required YukiErrorCode Code { get; init; } = YukiErrorCode.Unknown;

    public string? Message { get; init; }

    [JsonIgnore]
    public bool Ok => Code == YukiErrorCode.Ok;
}

public sealed class YukiResponse<TData> : YukiResponse where TData : class
{
    public TData Data { get; init; } = null!;
}