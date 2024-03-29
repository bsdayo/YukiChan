﻿namespace YukiChan.Shared.Utils;

public static class DateTimeUtils
{
    public static string ToDisplayText(this DateTime dateTime, bool toUtc8 = false)
    {
        return (toUtc8 ? dateTime.AddHours(8) : dateTime).ToString("yyyy.MM.dd HH:mm:ss");
    }

    public static string FormatUtc8Text(long timestamp, bool inMilliseconds = false)
    {
        return (inMilliseconds
                ? DateTimeOffset.FromUnixTimeMilliseconds(timestamp)
                : DateTimeOffset.FromUnixTimeSeconds(timestamp))
            .UtcDateTime
            .AddHours(8)
            .ToDisplayText();
    }

    public static long GetTimestamp(this DateTime dateTime, bool inMilliseconds = false, bool inUtc8 = false)
    {
        var ts = (inUtc8 ? dateTime.AddHours(-8) : dateTime) - DateTime.UnixEpoch;
        return inMilliseconds
            ? (long)ts.TotalMilliseconds
            : (long)ts.TotalSeconds;
    }

    public static string GetPastDays(this DateTime dateTime, long timestamp, bool inMilliseconds = false)
    {
        return inMilliseconds
            ? $"{(int)TimeSpan.FromMilliseconds(dateTime.GetTimestamp(true) - timestamp).TotalDays}d"
            : $"{(int)TimeSpan.FromSeconds(dateTime.GetTimestamp() - timestamp).TotalDays}d";
    }
}