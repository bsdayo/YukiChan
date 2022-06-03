using System;

namespace YukiChan.Utils;

public static class CommonUtils
{
    public static double Bytes2MiB(this long bytes, int round)
        => Math.Round(bytes / 1048576.0, round);
}