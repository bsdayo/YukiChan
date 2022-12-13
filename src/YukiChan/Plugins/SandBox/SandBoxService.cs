using System.Text;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Logging;

namespace YukiChan.Plugins.SandBox;

public sealed class SandBoxService
{
    private ScriptState<object?>? _scriptState;

    private readonly ScriptOptions _scriptOptions;

    private readonly ILogger<SandBoxPlugin> _logger;

    private readonly string[] _references =
    {
        "System.Runtime",
        "System.Collections",
        "System.Collections.Concurrent",
        "System.Text.Encoding",
        "System.ValueTuple",
        "System.Linq"
    };

    private readonly string[] _imports =
    {
        "System",
        "System.Math",
        "System.Collections.Generic",
        "System.Linq"
    };

    private readonly string[] _bannedKeywords =
    {
        "System.Threading",
        "System.IO",
        "System.Net",
        "Microsoft.CodeAnalysis",
        "System.Diagnostics",
        "System.Drawing",
        "System.Reflection",
        "System.Resources",
        "System.Runtime",
        "System.Windows",
        "WindowsBase",
        "GetType",
        "typeof",
        "Pow",
        "Environment"
    };

    public Exception? LastException { get; private set; }

    public SandBoxService(ILogger<SandBoxPlugin> logger)
    {
        _logger = logger;

        _scriptOptions = ScriptOptions.Default
            .WithFileEncoding(Encoding.UTF8)
            .WithReferences(_references)
            .WithImports(_imports);
    }

    private async Task<ScriptState<object?>> GetScriptState()
    {
        if (_scriptState is not null)
            return _scriptState;

        return _scriptState = await CSharpScript
            .Create<object?>(string.Empty, _scriptOptions)
            .RunAsync(null, _ => true);
    }

    public void Reset() => _scriptState = null;

    public async Task<object?> Execute(string code, TimeSpan timeout)
    {
        if (_bannedKeywords.Any(code.Contains)) return null;
        if (double.TryParse(code, out _)) return null;

        var cts = new CancellationTokenSource(timeout);

        try
        {
            var state = await GetScriptState();
            _scriptState = await state.ContinueWithAsync(code, _scriptOptions, cts.Token);
        }
        catch (Exception e)
        {
            LastException = e;
            return null;
        }

        _logger.LogInformation("Type: {Type}, Value: {Value}",
            _scriptState?.ReturnValue?.GetType().FullName ?? "<null>",
            _scriptState?.ReturnValue);

        return _scriptState?.ReturnValue;
    }
}