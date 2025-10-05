open Microsoft.Extensions.Logging
open System
open System.IO
open System.Threading

type OutputType =
    | Console
    | File
    | ConsoleAndFile

type Logger(outputType: OutputType) =
    let lockObj = obj()
    let mutable fileWriter: StreamWriter option = None
    
    // Default properties with mutable for C# compatibility
    member val InfoColor = ConsoleColor.Green with get, set
    member val WarnColor = ConsoleColor.Yellow with get, set
    member val ErrorColor = ConsoleColor.Red with get, set
    member val DebugColor = ConsoleColor.Magenta with get, set
    member val TimeColor = ConsoleColor.White with get, set
    member val FatalColor = ConsoleColor.Red with get, set
    member val IncludeCallStack = false with get, set
    member val LogOutputType = outputType with get
    
    // Default constructor
    new() = Logger(OutputType.Console)
    
    do
        // Initialize file writer if needed
        match outputType with
        | File | ConsoleAndFile ->
            let logFilePath = Path.Combine(Environment.CurrentDirectory, $"{DateTime.Now:yyyy-MM-dd}.log")
            fileWriter <- Some(new StreamWriter(logFilePath, append = true, AutoFlush = true))
        | _ -> ()
    
    let getLogLevelOutput (level: LogLevel) =
        match level with
        | LogLevel.Critical -> "FATAL"
        | LogLevel.Error -> "ERROR"
        | LogLevel.Warning -> "WARN"
        | LogLevel.Information -> "INFO"
        | LogLevel.Debug -> "DEBUG"
        | LogLevel.Trace -> "TRACE"
        | _ -> "UNKN"
    
    let getLogLevelColor (level: LogLevel) =
        match level with
        | LogLevel.Critical -> FatalColor
        | LogLevel.Error -> ErrorColor
        | LogLevel.Warning -> WarnColor
        | LogLevel.Information -> InfoColor
        | LogLevel.Debug -> DebugColor
        | _ -> InfoColor
    
    let writeToConsole (level: LogLevel) (message: string) =
        let originalColor = Console.ForegroundColor
        
        Console.ForegroundColor <- TimeColor
        Console.Write($"[{DateTime.Now:HH:mm:ss.fff}] ")
        
        Console.ForegroundColor <- getLogLevelColor level
        Console.Write($"{getLogLevelOutput level} ")
        
        Console.ForegroundColor <- originalColor
        Console.WriteLine(message)
    
    let writeToFile (message: string) =
        fileWriter |> Option.iter (fun writer -> writer.WriteLine(message))
    
    let log (logLevel: LogLevel) (message: string) =
        if not (isEnabled logLevel) then () else
        
        let finalMessage =
            if (logLevel = LogLevel.Error || logLevel = LogLevel.Critical) && IncludeCallStack then
                message + $"\nCall stack: {Environment.StackTrace}"
            else
                message
        
        lock lockObj (fun () ->
            match LogOutputType with
            | Console | ConsoleAndFile -> writeToConsole logLevel finalMessage
            | _ -> ()
            
            match LogOutputType with
            | File | ConsoleAndFile -> writeToFile finalMessage
            | _ -> ()
        )
    
    let isEnabled (_: LogLevel) = true
    
    interface ILogger with
        member this.Log<'TState>(logLevel: LogLevel, eventId: EventId, state: 'TState, 
                                exception': exn, formatter: Func<'TState, exn, string>) =
            if not (isEnabled logLevel) then () else
            let message = formatter.Invoke(state, exception')
            log logLevel message
        
        member this.IsEnabled(logLevel: LogLevel) = isEnabled logLevel
        
        member this.BeginScope<'TState>(state: 'TState) when 'TState : notnull = 
            NullScope.Instance :> IDisposable
    
    interface IDisposable with
        member this.Dispose() =
            lock lockObj (fun () ->
                fileWriter |> Option.iter (fun writer -> writer.Dispose())
                fileWriter <- None
                GC.SuppressFinalize(this)
            )

and private NullScope() =
    static member val Instance = NullScope() :> IDisposable
    interface IDisposable with
        member _.Dispose() = ()