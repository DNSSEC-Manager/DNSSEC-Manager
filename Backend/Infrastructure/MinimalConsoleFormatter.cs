using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace Backend.Infrastructure;

public class MinimalConsoleFormatter : ConsoleFormatter
{
    public MinimalConsoleFormatter() : base("Minimal") { }

    public override void Write<TState>(in LogEntry<TState> logEntry,
        IExternalScopeProvider scopeProvider,
        TextWriter textWriter)
    {
        var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception) ?? string.Empty;

        if (string.IsNullOrEmpty(message) && logEntry.Exception != null)
            message = logEntry.Exception.ToString();

        if (string.IsNullOrEmpty(message))
            return;
        
        textWriter.WriteLine(message);
    }
}