using McMaster.Extensions.CommandLineUtils;
using System.IO.Compression;
using System.Text;
using SQLite;
using YukiChan.Modules.Arcaea.Models;

namespace YukiChan.Tools.Commands.Arcaea;

[Command(
    Name = "arcaea",
    FullName = "Arcaea",
    Description = "Arcaea Tools")]
[Subcommand(
    typeof(UnpackAssets),
    typeof(UpdateSongDb))]
public class ArcaeaCommand
{
    public void OnExecute(CommandLineApplication app)
    {
        app.ShowHelp();
    }
}