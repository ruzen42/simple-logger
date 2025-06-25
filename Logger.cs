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
    }

    public ConsoleColor InfoColor;
    public ConsoleColor WarnColor;
    public ConsoleColor ErrorColor;
    public ConsoleColor DebugColor;
    public ConsoleColor TimeColor;
    public ConsoleColor FatalColor;
    public TypeLogger _typeLogger;
    public bool CallStack {
        get;
        set;
    }

    public Logger(TypeLogger typeLogger)
    {
        CallStack = false;
        _typeLogger = typeLogger;
        InfoColor = ConsoleColor.Green;
        WarnColor = ConsoleColor.Yellow;
        ErrorColor = ConsoleColor.Red;
        DebugColor = ConsoleColor.Magenta;
        TimeColor = ConsoleColor.White;
        FatalColor = ConsoleColor.Red;
        Info("Logging started");
    }

    public Logger(TypeLogger typeLogger, ConsoleColor  infoColor, ConsoleColor  warnColor, ConsoleColor  errorColor, ConsoleColor  debugColor, ConsoleColor  timeColor, ConsoleColor  fatalColor)
    {
        _typeLogger = typeLogger;
        InfoColor = infoColor;
        WarnColor = warnColor;
        ErrorColor = errorColor;
        DebugColor = debugColor;
        TimeColor = timeColor;
        FatalColor = fatalColor;
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
            case TypeLogger.ConsoleAndFile:
            case TypeLogger.Console:
            {
                Console.ForegroundColor = TimeColor;
                Console.Write($"[{DateTime.Now:HH:mm:ss.fff}] ");
                Console.ForegroundColor = type switch
                {
                  TypeMessage.Info => InfoColor,
                  TypeMessage.Error => ErrorColor,
                  TypeMessage.Fatal => FatalColor,
                  TypeMessage.Debug => DebugColor,
                  TypeMessage.Warn => WarnColor,
                  _ => InfoColor
                };
                Console.Write($"{level,-5} ");
                Console.ForegroundColor = originalColor;
                Console.WriteLine(message);
                if (level is not ("ERROR" or "FATAL") || !CallStack) break;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"Call stack: {Environment.StackTrace}\n");
                Console.ForegroundColor = originalColor;
                break;
            }
            case TypeLogger.File:
            {
                var streamWriter = new StreamWriter($"{Environment.CurrentDirectory}\\{DateTime.Now:yyyy-MM-dd}.log");
                streamWriter.Write($"[{DateTime.Now:HH:mm:ss.fff}] ");
                Console.ForegroundColor = type switch
                {
                    TypeMessage.Info => InfoColor,
                    TypeMessage.Error => ErrorColor,
                    TypeMessage.Fatal => FatalColor,
                    TypeMessage.Debug => DebugColor,
                    TypeMessage.Warn => WarnColor,
                    _ => InfoColor
                };
                streamWriter.Write($"{level,-5} ");
                streamWriter.WriteLine(message);
                if (level is ("ERROR" or "WARN") || !CallStack)
                    streamWriter.WriteLine($"Call stack: {Environment.StackTrace}\n");
                break;              
            }

        default:
            throw new ArgumentOutOfRangeException();
        }
    }

}
