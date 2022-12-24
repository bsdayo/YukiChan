using YukiChan.Shared.Utils;

namespace YukiChan.Tests.UtilsTests;

public class DateTimeUtilsTests
{
    [Theory]
    [InlineData(1671925077008, true, "2022.12.25 07:37:57")]
    [InlineData(1671925077, false, "2022.12.25 07:37:57")]
    public void TestFormatUtc8Text(long timestamp, bool inMilliseconds, string expected)
    {
        Assert.Equal(expected, DateTimeUtils.FormatUtc8Text(timestamp, inMilliseconds));
    }

    [Theory]
    [InlineData(2022, 12, 24, 23, 37, 57, 8, true, 1671925077008)]
    [InlineData(2022, 12, 24, 23, 37, 57, 8, false, 1671925077)]
    public void TestGetTimestamp(int year, int month, int day, int hour, int minute, int second, int millisecond,
        bool inMilliseconds, long expected)
    {
        var dt = new DateTime(year, month, day, hour, minute, second, millisecond);
        Assert.Equal(expected, dt.GetTimestamp(inMilliseconds));
    }

    [Theory]
    [InlineData(1607776496000, true, "742d")]
    [InlineData(1607776496, false, "742d")]
    public void TestGetPastDays(long timestamp, bool inMilliseconds, string expected)
    {
        var now = new DateTime(2022, 12, 25, 6, 3, 6);
        Assert.Equal(expected, now.GetPastDays(timestamp, inMilliseconds));
    }
}