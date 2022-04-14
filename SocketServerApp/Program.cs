using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SocketShared;
using SocketShared.Config;
using SocketShared.Events.Args;
using SocketShared.Handlers;

SocketShared.Socket.SocketServer _socketServer;

try
{
    DiContainer.Init();
    
    Log.Logger.Information("---- Создание Socket сервера ---- ");

    Log.Logger.Information("Получение конфигов...");

    var iniConfig = new IniConfig("config.ini");

    Log.Logger.Information("Конфиги успешн ополучены");

    _socketServer = DiContainer.Services.GetRequiredService<SocketShared.Socket.SocketServer>();
    // _socketServer.Handlers.ClientConnectedHandler<ClientConnectedHandler>();
    
    _socketServer.Events.ClientConnected += OnClientConnected;
    _socketServer.Events.ClientDisconnected += OnClientDisconnected;
    _socketServer.Events.DataReceive += OnDataReceive;

    _socketServer.Create(iniConfig.Ip, iniConfig.Port);
    _socketServer.Start();

    while (true)
    {
        var line = Console.ReadLine();
        if (line == "exit")
        {
            Log.Logger.Information("Завершение работы клиента");
            Environment.Exit(0);
        }
    }
}
catch (Exception ex)
{
    Log.Logger.Error($"Ошибка в сервере: {ex}");
}

void OnDataReceive(object? sender, DataReceiveEventArgs e)
{
    Log.Logger.Information($"Новые данные");
}

void OnClientDisconnected(object? sender, ClientDisconnectedEventArgs e)
{
    Log.Logger.Information($"Клиент отключился");
}

void OnClientConnected(object? sender, ClientConnectedEventArgs e)
{
    Log.Logger.Information($"Клиент {e.ClientIp} подключился");
}