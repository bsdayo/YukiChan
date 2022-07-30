namespace YukiChan.Modules.Maimai;

public static class MaimaiUtils
{
    public static string GetChart4DigitId(int chartId)
    {
        if (chartId > 10000)
            chartId -= 10000;
        return chartId.ToString("D4");
    }

    public static string GetGradeText(string grade)
    {
        return grade switch
        {
            "sp" => "S+",
            "ssp" => "SS+",
            "sssp" => "SSS+",
            _ => grade.ToUpper()
        };
    }

    public static string GetHonorText(string honor)
    {
        return honor switch
        {
            "fcp" => "FC+",
            "fsp" => "FS+",
            "app" => "AP+",
            _ => honor.ToUpper()
        };
    }
}