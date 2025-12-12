using Microsoft.Extensions.Logging;

namespace NeoSimpleLogger;

public class SimpleLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new SimpleLogger();

    public void Dispose() {}
}
