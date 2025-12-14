using Microsoft.Extensions.Logging;
using System;

namespace NeoSimpleLogger;

public class SimpleLogger(SimpleLogger.TypeLogger type) : ILogger
{
    public enum TypeLogger
    {
        Console,
        File,
        ConsoleAndFile
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var time = TimeOnly.FromDateTime(dateTime: new DateTime().Date.Date);
        Console.Write("[");
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        throw new NotImplementedException();
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }
}
