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

    private readonly ConsoleColor _infoColor;
    private readonly ConsoleColor _warnColor;
    private readonly ConsoleColor _errorColor;
    private readonly ConsoleColor _debugColor;
    private readonly ConsoleColor _timeColor;
    private TypeLogger _typeLogger;

    public Logger(TypeLogger typeLogger)
    {
        _typeLogger = typeLogger;
        _infoColor = ConsoleColor.Green;
        _warnColor = ConsoleColor.Yellow;
        _errorColor = ConsoleColor.Red;
        _debugColor = ConsoleColor.Magenta;
        _timeColor = ConsoleColor.DarkGray;
        Info("Logging started");
    }
    
    public Logger(TypeLogger typeLogger, ConsoleColor infoColor, ConsoleColor warnColor, ConsoleColor errorColor, ConsoleColor debugColor, ConsoleColor timeColor)
    {
        _typeLogger = typeLogger;
        _infoColor = infoColor;
        _warnColor = warnColor;
        _errorColor = errorColor;
        _debugColor = debugColor;
        _timeColor = timeColor;
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
        Console.ForegroundColor = type switch
        {
            TypeMessage.Info => _infoColor,
            TypeMessage.Error => _errorColor,
            TypeMessage.Debug => _debugColor,
            TypeMessage.Warn => _warnColor,
            _ => _infoColor
        };

        var level = type switch
        {
            TypeMessage.Info => "INFO",
            TypeMessage.Error => "ERROR",
            TypeMessage.Debug => "DEBUG",
            TypeMessage.Warn => "WARN",
            _ => "INFO"
        };

       switch (_typeLogger) 
       {
           case TypeLogger.Console:
            Console.ForegroundColor = _timeColor;
            Console.Write($"[{DateTime.Now:HH:mm:ss.fff}] ");
            Console.Write($"{level,-5} ");
            Console.ForegroundColor = originalColor;
            Console.WriteLine(message);
#if DEBUG
            if (level is not ("ERROR" or "WARN")) return;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"Call stack: {Environment.StackTrace}\n");
            Console.ForegroundColor = originalColor;
#endif
            break; 
           case TypeLogger.File:
            var streamWriter = new StreamWriter($"{Environment.CurrentDirectory}\\{DateTime.Now:yyyy-MM-dd}.log");
            streamWriter.Write($"[{DateTime.Now:HH:mm:ss.fff}] ");
            streamWriter.Write($"{level,-5} ");
            streamWriter.WriteLine(message);
#if DEBUG
            if (level is ("ERROR" or "WARN"))
            {
                streamWriter.WriteLine($"Call stack: {Environment.StackTrace}\n");
            }
#endif
            break; 
           case TypeLogger.ConsoleAndFile:
               Console.ForegroundColor = _timeColor;
               Console.Write($"[{DateTime.Now:HH:mm:ss.fff}] ");
               Console.Write($"{level,-5} ");
               Console.ForegroundColor = originalColor;
               Console.WriteLine(message);
#if DEBUG
               if (level is not ("ERROR" or "WARN")) return;

               Console.ForegroundColor = ConsoleColor.DarkGray;
               Console.WriteLine($"Call stack: {Environment.StackTrace}\n");
               Console.ForegroundColor = originalColor;
#endif
               goto case TypeLogger.File;
       }
    }

}
