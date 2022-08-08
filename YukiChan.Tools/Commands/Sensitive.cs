using System.IO.Compression;
using System.Text;
using McMaster.Extensions.CommandLineUtils;
using YukiChan.Utils;

namespace YukiChan.Tools.Commands;

[Command(
    Name = "sensitive",
    FullName = "Sensitive Words",
    Description = "")]
[Subcommand(
    typeof(CheckSensitive),
    typeof(ReplaceSensitive))]
[HelpOption]
public class SensitiveCommand
{
    public void OnExecute(CommandLineApplication app)
    {
        app.ShowHelp();
    }

    [Command(
        Name = "check",
        FullName = "Check sensitive words",
        Description = "Check sensitive words")]
    [HelpOption]
    public class CheckSensitive
    {
        [Argument(0, "TEXT", "The text to check")]
        public string? Text { get; set; } = null;

        public void OnExecute(CommandLineApplication app)
        {
            if (string.IsNullOrWhiteSpace(Text))
            {
                app.ShowHelp();
                return;
            }

            SensitiveWordsFilter.Initialize();

            Console.WriteLine(Text.HasSensitiveWords());
        }
    }

    [Command(
        Name = "replace",
        FullName = "Replace sensitive words",
        Description = "Replace sensitive words")]
    [HelpOption]
    public class ReplaceSensitive
    {
        [Argument(0, "TEXT", "The text to replace")]
        public string? Text { get; set; } = null;

        public void OnExecute(CommandLineApplication app)
        {
            if (string.IsNullOrWhiteSpace(Text))
            {
                app.ShowHelp();
                return;
            }

            SensitiveWordsFilter.Initialize();

            Console.WriteLine(Text.ReplaceSensitiveWords());
        }
    }
}