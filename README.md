# NeoSimpleLogger README

## 📦 Overview
NeoSimpleLogger is a lightweight, color-coded logging utility for .NET applications. It provides structured logging with timestamps, customizable colors for different log levels, and
optional stack trace output in debug mode. Perfect for debugging and monitoring application behavior.

---

## 🎨 Features
- **Color-Coded Log Levels**:
    - `INFO` (Green)
    - `WARN` (Yellow)
    - `ERROR` (Red)
    - `DEBUG` (Magenta)
- **Customizable Colors**: Define your own color scheme for each log level.
- **Timestamps**: Automatically adds the current time in `HH:mm:ss.fff` format.
- **Debug Stack Trace**: In `DEBUG` mode, shows the call stack for non-`ERROR`/`WARN` logs.

---

## 📌 Usage
### 1. **Default Logger**
```csharp
using NeoSimpleLogger;

var logger = new Logger();
logger.Info("Application started");
logger.Warn("Low memory detected");
logger.Error("Critical failure occurred");
logger.Debug("Debugging internal state");
```

### 2. **Custom Colors**
```csharp
var logger = new Logger(
    infoColor: ConsoleColor.Cyan,
    warnColor: ConsoleColor.DarkYellow,
    errorColor: ConsoleColor.DarkRed,
    debugColor: ConsoleColor.DarkMagenta,
    timeColor: ConsoleColor.Gray
);
```

---

## 📋 Example Output
```
[14:30:45.123] INFO   Application started
[14:30:45.124] WARN   Low memory detected
[14:30:45.125] ERROR  Critical failure occurred
[14:30:45.126] DEBUG  Debugging internal state
```

---

## 🔍 Debug Mode (Optional)
In `DEBUG` builds, the logger includes the call stack for non-`ERROR`/`WARN` logs:
```
[14:30:45.126] DEBUG  Debugging internal state
Call stack:    at NeoSimpleLogger.Logger.Debug(String message)
               at MyApplication.Main(String[] args)
```

---

## ⚠️ Notes
- **Color Support**: Requires a terminal or console that supports ANSI escape codes.
- **Performance**: Minimal overhead due to simple formatting and no external dependencies.
- **License**: MIT License (see `LICENSE` file).

---

## 📦 Installation
Add the `NeoSimpleLogger` namespace to your project. No external packages required.

---

## 📚 License
[MIT License](https://opensource.org/licenses/MIT)  
© 2023 Your Name or Organization

---

## 🛠️ Contributing
Pull requests and issues are welcome!
- Report bugs: [GitHub Issues](https://github.com/yourusername/NeoSimpleLogger/issues)
- Feature requests: [GitHub Discussions](https://github.com/yourusername/NeoSimpleLogger/discussions)  

