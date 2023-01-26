using System.Net;
using YukiChan.Shared.Data;

namespace YukiChan.Client.Console;

public sealed class YukiApiException : Exception
{
    public YukiErrorCode ErrorCode { get; }

    public HttpStatusCode StatusCode { get; }

    internal YukiApiException(HttpStatusCode statusCode, YukiErrorCode errCode, string message) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errCode;
    }

    internal YukiApiException(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
        ErrorCode = YukiErrorCode.Unknown;
    }
}