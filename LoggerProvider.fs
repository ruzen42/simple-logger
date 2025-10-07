namespace NeoSimpleLogger

open System
open System.Collections.Concurrent
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Options

type OutputType =
    | Console = 0
    | File = 1

[<CLIMutable>]
type LoggerOptions() =
    member val OutputType = OutputType.Console with get, set

type Logger(categoryName: string, options: LoggerOptions) =
    interface ILogger with
        member _.Log(logLevel, eventId, state, exception, formatter) =
            ()
        member _.IsEnabled(logLevel) = true
        member _.BeginScope<'TState>(state: 'TState) =
            { new IDisposable with member _.Dispose() = () }


[<Sealed>]
type LoggerProvider =
    let _loggers = new ConcurrentDictionary<string, ILogger>()
    let mutable _currentOptions: LoggerOptions = null
    let mutable _onChangeToken: IDisposable option = None

    private new(initialOptions: LoggerOptions, onChangeToken: IDisposable option) =
        { new LoggerProvider() with
            _currentOptions = initialOptions
            _onChangeToken = onChangeToken
        }

    new(optionsMonitor: IOptionsMonitor<LoggerOptions>) =
        let initialOptions = optionsMonitor.CurrentValue
        let token =
            optionsMonitor.OnChange(fun updatedOptions _ ->
                _currentOptions <- updatedOptions
            )
        LoggerProvider(initialOptions, Some token)

    new(outputType: OutputType) =
        let initialOptions = LoggerOptions(OutputType = outputType)
        LoggerProvider(initialOptions, None)

    interface ILoggerProvider with
        member this.CreateLogger(categoryName: string) : ILogger =
            _loggers.GetOrAdd(
                categoryName,
                fun name ->
                    new Logger(name, _currentOptions) :> ILogger 
            )

        member this.Dispose() =
            _loggers.Clear()
            match _onChangeToken with
            | Some token -> token.Dispose()
            | None -> ()
            GC.SuppressFinalize(this)

