using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using SocketShared.Socket;

namespace SocketShared;

public class DiContainer
{
    private static ServiceProvider _services;
    public static ServiceProvider Services => _services;

    public static void Init()
    {
        SetUpLogger();
        
        _services = new ServiceCollection()
            .AddSingleton<SocketServer>()
            .BuildServiceProvider();
    }
    
    /// <summary>
    /// Настройка логирования
    /// </summary>
    private static void SetUpLogger()
    {
        var appDirectory = Environment.CurrentDirectory;
        var pathToLogsDir = Path.Combine(appDirectory, "Logs", "logs_socket_server");
        var logsExists = Directory.Exists(pathToLogsDir);

        if (!logsExists)
            Directory.CreateDirectory(pathToLogsDir);

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(
                theme: AnsiConsoleTheme.Code,
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                shared: true,
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: LogEventLevel.Error,
                retainedFileCountLimit: 10,
                path: pathToLogsDir + "/log-.txt")
            .CreateLogger();
    }
}