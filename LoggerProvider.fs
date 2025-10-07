namespace NeoSimpleLogger

open System
open System.Collections.Concurrent
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Options

[<Sealed>]
public type LoggerProvider =
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

