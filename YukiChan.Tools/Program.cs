using McMaster.Extensions.CommandLineUtils;
using YukiChan.Tools.Commands;
using YukiChan.Tools.Commands.Arcaea;

namespace YukiChan.Tools;

[Subcommand(typeof(ArcaeaCommand))]
[Subcommand(typeof(BackupDatabases))]
[HelpOption]
public class Program
{
    public static int Main(string[] args)
    {
        return CommandLineApplication.Execute<Program>(args);
    }

    private void OnExecute(CommandLineApplication app)
    {
        app.ShowHelp();
    }
}