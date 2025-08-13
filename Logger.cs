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

        if (outputType is not (OutputType.File or OutputType.ConsoleAndFile)) return;
        var logFilePath = Path.Combine(Environment.CurrentDirectory, $"{DateTime.Now:yyyy-MM-dd}.log");
        _fileWriter = new StreamWriter(logFilePath, append: true) { AutoFlush = true };
    }

    public Logger() => new Logger(OutputType.Console);

    private void Log(LogLevel logLevel, string message)
    {
        if (!IsEnabled(logLevel))
            return;


        
        if (logLevel is LogLevel.Error or LogLevel.Critical && IncludeCallStack)
        {
            message += $"\nCall stack: {Environment.StackTrace}";
        }

        lock (_lock)
        {
            if (LogOutputType is OutputType.Console or OutputType.ConsoleAndFile)
            {
                WriteToConsole(logLevel, message);
            }

            if (LogOutputType is OutputType.File or OutputType.ConsoleAndFile)
            {
                WriteToFile(message);
            }
        }
    }

    private void WriteToConsole(LogLevel level, string message)
    {
        var originalColor = Console.ForegroundColor;
        
        Console.ForegroundColor = TimeColor;
        Console.Write($"[{DateTime.Now:HH:mm:ss.fff}] ");
        
        Console.ForegroundColor = level switch
        {
            LogLevel.Critical => FatalColor, 
            LogLevel.Error => ErrorColor,
            LogLevel.Warning => WarnColor, 
            LogLevel.Information => InfoColor, 
            LogLevel.Debug => DebugColor, 
            _ => InfoColor
        };
        
        var output = level switch
        {
            LogLevel.Critical => "FATAL",
            LogLevel.Error => "ERROR",
            LogLevel.Warning => "WARN",
            LogLevel.Information => "INFO",
            LogLevel.Debug => "DEBUG",
            LogLevel.Trace => "TRACE",
            _ => "UNKN"
        };
        Console.Write(output + " ");
        Console.ForegroundColor = originalColor;
        
        Console.WriteLine(message);
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

    public IDisposable BeginScope<TState>(TState state) where TState : notnull => 
        NullScope.Instance;

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
