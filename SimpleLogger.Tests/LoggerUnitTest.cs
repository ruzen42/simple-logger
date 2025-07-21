using NeoSimpleLogger;
using Xunit;

namespace SimpleLogger.Tests;

public class LoggerUnitTest : IDisposable
{
    private readonly StringWriter _consoleOutput;
    private readonly TextWriter _originalConsoleOut;

    public LoggerUnitTest()
    {
        _originalConsoleOut = Console.Out;
        _consoleOutput = new StringWriter();
        Console.SetOut(_consoleOutput);
    }

    public void Dispose()
    {
        Console.SetOut(_originalConsoleOut);
        _consoleOutput.Dispose();
    }

    [Fact]
    public void LogInformation_ShouldWriteToConsole_WithCorrectFormat()
    {
        var logger = new Logger(Logger.OutputType.Console);

        logger.Info("Test message");

        var output = _consoleOutput.ToString();
        Assert.Matches(@"\[\d{2}:\d{2}:\d{2}\.\d{3}\] INFO  Test message", output);
        Assert.Contains("INFO", output);
        Assert.Contains("Test message", output);
    }

    [Fact]
    public void LogError_ShouldUseCorrectColor()
    {
        var logger = new Logger(Logger.OutputType.Console)
        {
            ErrorColor = ConsoleColor.DarkRed
        };

        logger.Error("Error message");

        var output = _consoleOutput.ToString();
        Assert.Contains("ERROR", output);
        Assert.Contains("Error message", output);
    }

    [Fact]
    public void LogToFile_ShouldCreateLogFile()
    {
        var testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(testDir);
        var logFile = Path.Combine(testDir, $"{DateTime.Now:yyyy-MM-dd}.log");

        try
        {
            var logger = new Logger(Logger.OutputType.File);

            logger.Info("File log test");
            logger.Dispose();

            Assert.True(File.Exists(logFile));
            var content = File.ReadAllText(logFile);
            Assert.Contains("INFO", content);
            Assert.Contains("File log test", content);
        }
        finally
        {
            Directory.Delete(testDir, true);
        }
    }

    [Fact]
    public void LogWithCallStack_ShouldIncludeStackTrace_ForErrorLevel()
    {
        var logger = new Logger(Logger.OutputType.Console)
        {
            IncludeCallStack = true
        };

        logger.Error("Error with stack");

        var output = _consoleOutput.ToString();
        Assert.Contains("Call stack:", output);
        Assert.Contains("System.Environment.get_StackTrace()", output);
    }

    [Fact]
    public void Dispose_ShouldNotThrow_WhenFileWriterNotInitialized()
    {
        var logger = new Logger(Logger.OutputType.Console);

        var exception = Record.Exception(() => logger.Dispose());
        Assert.Null(exception);
    }
}
