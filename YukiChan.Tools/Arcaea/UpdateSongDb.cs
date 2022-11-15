using Spectre.Console;
using Spectre.Console.Cli;

namespace YukiChan.Tools.Arcaea;

public class UpdateSongDbCommand : Command<UpdateSongDbCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "[Path]")]
        public string? Path { get; set; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        AnsiConsole.MarkupLine("Hello, [blue]b1acksoil[/]!");
        return 0;
    }
}