namespace YukiChan.Shared.Models.Arcaea;

// ReSharper disable InconsistentNaming
public enum ArcaeaDifficulty
{
    Past = 0,
    Present = 1,
    Future = 2,
    Beyond = 3
}

public enum ArcaeaClearType
{
    TrackLost = 0,
    NormalClear = 1,
    FullRecall = 2,
    PureMemory = 3,
    EasyClear = 4,
    HardClear = 5
}

public enum ArcaeaGrade
{
    D,
    C,
    B,
    A,
    AA,
    EX,
    EXP
}

public enum ArcaeaGuessMode
{
    Easy, // 简单
    Normal, // 正常
    Hard, // 困难
    Flash, // 闪照
    GrayScale, // 灰度
    Invert // 反色
}