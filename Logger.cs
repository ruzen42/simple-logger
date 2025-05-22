using System;

namespace RBoard;

public static class Logger
{
    private const ConsoleColor InfoColor = ConsoleColor.Green;
    private const ConsoleColor WarnColor = ConsoleColor.Yellow;
    private const ConsoleColor ErrorColor = ConsoleColor.Red;
    private const ConsoleColor DebugColor = ConsoleColor.Magenta;
    private const ConsoleColor TimeColor = ConsoleColor.DarkGray;

    public static void Error(string message)
    {
        Log("ERROR", message);
    }
    
    public static void Warn(string message)
    {
        Log("WARN", message);
    }
    
    public static void Debug(string message)
    {
        Log("DEBUG", message);
    }
    
    public static void Info(string message)
    {
        Log("INFO", message);
    }
    
    private static void Log(string level, string message)
    {
        var originalColor = Console.ForegroundColor;

        Console.ForegroundColor = TimeColor;
        Console.Write($"[{DateTime.Now:HH:mm:ss.fff}] ");

        Console.ForegroundColor = level switch
        {
            "INFO" => InfoColor,
            "ERROR" => ErrorColor,
            "DEBUG" => DebugColor,
            "WARN" => WarnColor,
            _ => InfoColor
        };
        
        Console.Write($"{level,-5} ");

        Console.ForegroundColor = originalColor;
        Console.WriteLine(message);

#if DEBUG
        if (level is "ERROR" or "WARN")
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"Call stack: {Environment.StackTrace}\n");
            Console.ForegroundColor = originalColor;
        }
#endif
    }
}
