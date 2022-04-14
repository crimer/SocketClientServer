using Serilog;
using SocketShared;
using SocketShared.Config;
using SocketShared.Socket;

try
{
    var tokenSource = new CancellationTokenSource();

    DiContainer.Init();

    Log.Logger.Information("Создание консольного Socket клиента");

    var iniConfig = new IniConfig("config.ini");

    var client = new SocketClient();
    await client.TryConnectAsync(iniConfig.Ip, iniConfig.Port, tokenSource.Token);

    while (true)
    {
        var line = Console.ReadLine();
        if (line == "exit")
        {
            Log.Logger.Information("Завершение работы сервера");
            Environment.Exit(0);
        }
    }
}
catch (Exception ex)
{
    Log.Logger.Error($"Ошибка в сервере: {ex}");
}