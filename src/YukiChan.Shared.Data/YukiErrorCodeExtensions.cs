using System.Text;
using static YukiChan.Shared.Data.YukiErrorCode;

namespace YukiChan.Shared.Data;

public static class YukiErrorCodeExtensions
{
    public static string GetMessage(this YukiErrorCode code)
    {
        return code switch
        {
            _ => code.FormatMessage()
        };
    }

    private static string FormatMessage(this YukiErrorCode code)
    {
        var span = code.ToString().AsSpan();
        var sb = new StringBuilder();

        var start = span.LastIndexOf('_') + 1;
        sb.Append(span[start]);
        for (var i = start + 1; i < span.Length; i++)
        {
            sb.Append(char.ToLower(span[i]));
            if (span.Length > i + 1 && char.IsUpper(span[i + 1]))
                sb.Append(' ');
        }

        return sb.ToString();
    }

    public static bool IsArcaeaAuaError(this YukiErrorCode errCode) =>
        errCode is < Arcaea_AuaErrorStart and > Arcaea_AuaErrorEnd;
}