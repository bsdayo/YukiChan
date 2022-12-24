using YukiChan.Shared.Utils;

namespace YukiChan.Tests.UtilsTests;

public class CommonUtilsTests
{
    [Theory]
    [InlineData(1024, 2, 0d)]
    [InlineData(114514, 4, 0.1092)]
    public void TestBytes2MiB(long bytes, int roundDigits, double expectedMiB)
    {
        Assert.Equal(expectedMiB, bytes.Bytes2MiB(roundDigits));
    }
}