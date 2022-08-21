using System.Dynamic;
using YukiChan.Utils;

namespace YukiChan.Core;

public class Argv
{
    public List<string> Arguments { get; set; } = new();

    public Dictionary<string, dynamic> Options { get; set; } = new();
    public string Source { get; init; } = "";
}

public class CommandParser
{
    public static Argv Parse(string source, Option[] options)
    {
        var argv = new Argv { Source = source };

        Option? nextOption = null;

        foreach (var section in source.Split(' '))
        {
            if (string.IsNullOrWhiteSpace(section)) continue;

            if (nextOption is not null)
            {
                switch (nextOption.Type)
                {
                    case OptionType.String:
                        argv.Options[nextOption.FullName] = section;

                        break;

                    case OptionType.Number:
                        argv.Options[nextOption.FullName] = double.TryParse(section, out var num)
                            ? num
                            : double.NaN;
                        break;
                }

                nextOption = null;
                continue;
            }

            Option? option = null;
            var boolAbbrChecked = false;
            if (section.StartsWith("--"))
            {
                option = options.FirstOrDefault(opt =>
                    !string.IsNullOrWhiteSpace(opt.FullName) && section.RemoveString("--") == opt.FullName);
            }
            else if (section.StartsWith("-"))
            {
                foreach (var opt in options)
                    if (opt.Type == OptionType.Boolean && section.RemoveString("-").Contains(opt.ShortName))
                        argv.Options[opt.FullName] = boolAbbrChecked = true;
                
                if (!boolAbbrChecked) option = options.FirstOrDefault(opt =>
                    !string.IsNullOrWhiteSpace(opt.ShortName) && section.RemoveString("-") == opt.ShortName);
            }

            if (option is not null)
            {
                if (option.Type == OptionType.Boolean)
                    argv.Options[option.FullName] = true;
                else
                    nextOption = option;
            }
            else if (!boolAbbrChecked)
            {
                argv.Arguments.Add(section);
            }
        }

        return argv;
    }
}