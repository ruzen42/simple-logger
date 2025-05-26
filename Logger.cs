namespace NeoSimpleLogger;

public class Logger
{
    private readonly ConsoleColor _infoColor;
    private readonly ConsoleColor _warnColor;
    private readonly ConsoleColor _errorColor;
    private readonly ConsoleColor _debugColor;
    private readonly ConsoleColor _timeColor;

    public Logger()
    {
       _infoColor = ConsoleColor.Green; 
       _warnColor = ConsoleColor.Yellow;
       _errorColor = ConsoleColor.Red;
       _debugColor = ConsoleColor.Magenta;
       _timeColor = ConsoleColor.DarkGray;
       Info("Logging started");
    }
    
    public Logger(ConsoleColor infoColor, ConsoleColor warnColor, ConsoleColor errorColor, ConsoleColor debugColor, ConsoleColor timeColor)
    {
        _infoColor = infoColor; 
        _warnColor = warnColor; 
        _errorColor = errorColor; 
        _debugColor = debugColor; 
        _timeColor = timeColor; 
        Info("Logging started");
    }

    public void Error(string message)
    {
        Log("ERROR", message);
    }

    public void Error(string message, ConsoleColor color)
    {
        Log("ERROR", message);
    }
    
    public void Warn(string message)
    {
        Log("WARN", message);
    }
    
    public void Debug(string message)
    {
        Log("DEBUG", message);
    }
    
    public void Info(string message)
    {
        Log("INFO", message);
    }
    
    private void Log(string level, string message)
    {
        var originalColor = Console.ForegroundColor;

        Console.ForegroundColor = _timeColor;
        Console.Write($"[{DateTime.Now:HH:mm:ss.fff}] ");

        Console.ForegroundColor = level switch
        {
            "INFO" => _infoColor,
            "ERROR" => _errorColor,
            "DEBUG" => _debugColor,
            "WARN" => _warnColor,
            _ => _infoColor
        };
        
        Console.Write($"{level,-5} ");

        Console.ForegroundColor = originalColor;
        Console.WriteLine(message);

#if DEBUG
        if (level is not ("ERROR" or "WARN")) return;
        
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"Call stack: {Environment.StackTrace}\n");
        Console.ForegroundColor = originalColor;
#endif
    }
}
