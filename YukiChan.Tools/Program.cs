using McMaster.Extensions.CommandLineUtils;
using YukiChan.Tools.Commands;

namespace YukiChan.Tools;

[Subcommand(typeof(ArcaeaCommand))]
[HelpOption]
public class Program
{
    public static int Main(string[] args)
        => CommandLineApplication.Execute<Program>(args);
    
    private void OnExecute(CommandLineApplication app)
    {
        app.ShowHelp();
    }
}