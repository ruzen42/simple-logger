namespace NeoSimpleLogger;

public class Logger
{
    public enum TypeLogger
    {
        Console,
        File,
        ConsoleAndFile,
    }

    private enum TypeMessage
    {
        Warn,
        Error,
        Info,
        Debug,
        Fatal,
        Your,
    }

    private readonly ConsoleColor _infoColor;
    private readonly ConsoleColor _warnColor;
    private readonly ConsoleColor _errorColor;
    private readonly ConsoleColor _debugColor;
    private readonly ConsoleColor _timeColor;
    private readonly ConsoleColor _fatalColor;
    private readonly TypeLogger _typeLogger;
    public bool CallStack {
        get;
        set;
    }

    public Logger(TypeLogger typeLogger)
    {
        CallStack = true;
        _typeLogger = typeLogger;
        _infoColor = ConsoleColor.Green;
        _warnColor = ConsoleColor.Yellow;
        _errorColor = ConsoleColor.Red;
        _debugColor = ConsoleColor.Magenta;
        _timeColor = ConsoleColor.DarkGray;
        _fatalColor = ConsoleColor.Red;
        Info("Logging started");
    }

    public Logger(TypeLogger typeLogger, ConsoleColor  infoColor, ConsoleColor  warnColor, ConsoleColor  errorColor, ConsoleColor  debugColor, ConsoleColor  timeColor, ConsoleColor  fatalColor)
    {
        _typeLogger = typeLogger;
        _infoColor = infoColor;
        _warnColor = warnColor;
        _errorColor = errorColor;
        _debugColor = debugColor;
        _timeColor = timeColor;
        _fatalColor = fatalColor;
        Info("Logging started");
    }

    public void Error(string message) => Log(TypeMessage.Error, message);

    public void Fatal(string message) => Log(TypeMessage.Fatal, message);

    public void Warn(string message) => Log(TypeMessage.Warn, message);

    public void Debug(string message) => Log(TypeMessage.Debug, message);

    public void Info(string message) => Log(TypeMessage.Info, message);

    private void Log(TypeMessage type, string message)
    {

        var originalColor = Console.ForegroundColor;
        var level = type switch
        {
          TypeMessage.Info => "INFO",
          TypeMessage.Error => "ERROR",
          TypeMessage.Fatal => "FATAL",
          TypeMessage.Debug => "DEBUG",
          TypeMessage.Warn => "WARN",
          _ => "YOUR"
        };

        switch (_typeLogger)
        {
          case TypeLogger.Console:
            Console.ForegroundColor = _timeColor;
            Console.Write($"[{DateTime.Now:HH:mm:ss.fff}] ");
            Console.ForegroundColor = type switch
            {
              TypeMessage.Info => _infoColor,
              TypeMessage.Error => _errorColor,
              TypeMessage.Fatal => _fatalColor,
              TypeMessage.Debug => _debugColor,
              TypeMessage.Warn => _warnColor,
              _ => _infoColor
            };
            Console.Write($"{level,-5} ");
            Console.ForegroundColor = originalColor;
            Console.WriteLine(message);
            if (level is not ("ERROR" or "FATAL") || !CallStack) break;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"Call stack: {Environment.StackTrace}\n");
            Console.ForegroundColor = originalColor;
            break;
        case TypeLogger.File:
            var streamWriter = new StreamWriter($"{Environment.CurrentDirectory}\\{DateTime.Now:yyyy-MM-dd}.log");
            streamWriter.Write($"[{DateTime.Now:HH:mm:ss.fff}] ");
            Console.ForegroundColor = type switch
            {
                TypeMessage.Info => _infoColor,
                TypeMessage.Error => _errorColor,
                TypeMessage.Fatal => _fatalColor,
                TypeMessage.Debug => _debugColor,
                TypeMessage.Warn => _warnColor,
                _ => _infoColor
            };
            streamWriter.Write($"{level,-5} ");
            streamWriter.WriteLine(message);
            if (level is ("ERROR" or "WARN") || !CallStack)
                streamWriter.WriteLine($"Call stack: {Environment.StackTrace}\n");
            break;
        case TypeLogger.ConsoleAndFile:
            Console.ForegroundColor = _timeColor;
            Console.Write($"[{DateTime.Now:HH:mm:ss.fff}] ");
            Console.ForegroundColor = type switch
            {
                TypeMessage.Info => _infoColor,
                TypeMessage.Error => _errorColor,
                TypeMessage.Fatal => _fatalColor,
                TypeMessage.Debug => _debugColor,
                TypeMessage.Warn => _warnColor,
                _ => _infoColor
            };
            Console.Write($"{level,-5} ");
            Console.ForegroundColor = originalColor;
            Console.WriteLine(message);
            if (level is not ("ERROR" or "WARN" or "FATAL") || !CallStack) return;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"Call stack: {Environment.StackTrace}\n");
            Console.ForegroundColor = originalColor;
        goto case TypeLogger.File;
        }
    }

}
