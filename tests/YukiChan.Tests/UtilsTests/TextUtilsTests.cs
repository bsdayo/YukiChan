using YukiChan.Shared.Utils;

namespace YukiChan.Tests.UtilsTests;

public class TextUtilsTests
{
    [Theory]
    [InlineData("Get Abbreviation", "GA")]
    [InlineData("toaster Koishi", "tK")]
    [InlineData(" da v in ci  _  ", "dvic_")]
    public void TestGetAbbreviation(string source, string expected)
    {
        Assert.Equal(expected, source.GetAbbreviation());
    }

    [Theory]
    [InlineData("test-remove-string", "st", "te-remove-ring")]
    [InlineData("asd_dsa", "d_d", "assa")]
    public void TestRemoveString(string source, string removal, string expected)
    {
        Assert.Equal(expected, source.RemoveString(removal));
    }
}