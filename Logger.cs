using Microsoft.Extensions.Logging;
using static System.GC;

namespace NeoSimpleLogger;

public class Logger : ILogger
{
    public enum OutputType
    {
        Console,
        File,
        ConsoleAndFile,
    }

    private readonly Lock _lock = new();
    private readonly StreamWriter? _fileWriter;

    public ConsoleColor InfoColor { get; set; } = ConsoleColor.Green;
    public ConsoleColor WarnColor { get; set; } = ConsoleColor.Yellow;
    public ConsoleColor ErrorColor { get; set; } = ConsoleColor.Red;
    public ConsoleColor DebugColor { get; set; } = ConsoleColor.Magenta;
    public ConsoleColor TimeColor { get; set; } = ConsoleColor.White;
    public ConsoleColor FatalColor { get; set; } = ConsoleColor.Red;
    public bool IncludeCallStack { get; set; }
    public OutputType LogOutputType { get; }

    public Logger(OutputType outputType)
    {
        LogOutputType = outputType;

        if (outputType is OutputType.File or OutputType.ConsoleAndFile)
        {
            var logFilePath = Path.Combine(Environment.CurrentDirectory, $"{DateTime.Now:yyyy-MM-dd}.log");
            _fileWriter = new StreamWriter(logFilePath, append: true) { AutoFlush = true };
        }

    }

    private void Log(LogLevel logLevel, string message)
    {
        if (!IsEnabled(logLevel))
            return;

        var level = logLevel switch
        {
            LogLevel.Trace => "TRACE",
            LogLevel.Debug => "DEBUG",
            LogLevel.Information => "INFO",
            LogLevel.Warning => "WARN",
            LogLevel.Error => "ERROR",
            LogLevel.Critical => "FATAL",
            _ => "UNKN"
        };

        var color = logLevel switch
        {
            LogLevel.Information => InfoColor,
            LogLevel.Warning => WarnColor,
            LogLevel.Error => ErrorColor,
            LogLevel.Critical => FatalColor,
            LogLevel.Debug => DebugColor,
            _ => ConsoleColor.Gray
        };

        var logMessage = $"{level,-5} {message}";
        
        if (logLevel is LogLevel.Error or LogLevel.Critical && IncludeCallStack)
        {
            logMessage += $"\nCall stack: {Environment.StackTrace}";
        }

        lock (_lock)
        {
            if (LogOutputType is OutputType.Console or OutputType.ConsoleAndFile)
            {
                WriteToConsole(logMessage, color);
            }

            if (LogOutputType is OutputType.File or OutputType.ConsoleAndFile)
            {
                WriteToFile(logMessage);
            }
        }
    }

    private void WriteToConsole(string message, ConsoleColor color)
    {
        var originalColor = Console.ForegroundColor;
        
        Console.ForegroundColor = TimeColor;
        Console.Write($"[{DateTime.Now:HH:mm:ss.fff}] ");
        
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        
        Console.ForegroundColor = originalColor;
    }

    private void WriteToFile(string message) => _fileWriter!.WriteLine(message);

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var message = formatter(state, exception);
        Log(logLevel, message);
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return NullScope.Instance;
    }

    private class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new();
        public void Dispose() { }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            _fileWriter?.Dispose();
#pragma warning disable CA1816
            SuppressFinalize(this);
#pragma warning restore CA1816
        }
    }
}