using System.Net;
using System.Net.Sockets;
using Serilog;
using SocketShared.Events;
using SocketShared.Handlers;

namespace SocketShared.Socket;

/// <summary>
/// Сервер
/// </summary>
public class SocketServer
{
    public ServerSocketEvents Events => _events;
    public bool IsListen => _isListen;
    public ServerHandlers Handlers { get; set; }

    private readonly ClientController _clientController;
    private readonly ServerSocketEvents _events;
    private System.Net.Sockets.Socket _serverSocket;
    private IPEndPoint _serverEndPoint;
    private bool _isListen;

    private CancellationTokenSource _tokenSource;
    private CancellationTokenSource _acceptConnectionsTokenSource;

    private CancellationToken _acceptConnectionsToken;
    private CancellationToken _token;

    private Task _acceptConnections;

    /// <summary>
    /// Конструктор
    /// </summary>
    public SocketServer()
    {
        _events = new ServerSocketEvents();
        _clientController = new ClientController();
        _isListen = false;
    }

    /// <summary>
    /// Создание сервера
    /// </summary>
    /// <param name="ip">IP</param>
    /// <param name="port">PORT</param>
    public void Create(string ip, int port)
    {
        if (!IPAddress.TryParse(ip, out var ipAddress))
            throw new InvalidOperationException("Не валидный IP адрес");

        _serverEndPoint = new IPEndPoint(ipAddress, port);

        _serverSocket = new System.Net.Sockets.Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
    }

    /// <summary>
    /// Остановка сервера
    /// </summary>
    public void Stop()
    {
        try
        {
            if (!_isListen)
            {
                Log.Logger.Information("Сокет сервер уже выключен");
                return;
            }

            _acceptConnectionsTokenSource.Cancel();
            _acceptConnections.Wait();
            _acceptConnections = null;
            _clientController.DisconnectAllClients();
            _serverSocket.Close();
            _serverSocket.Dispose();
            Log.Logger.Information("Сокет сервер выключен");
        }
        catch (Exception ex)
        {
            Log.Logger.Information($"Произошла ошибка при выключении сервера: {ex}");
        }
    }

    /// <summary>
    /// Запуск сервера
    /// </summary>
    public void Start()
    {
        try
        {
            if (_isListen)
            {
                Log.Logger.Information("Сокет сервер уже запущен");
                return;
            }

            if (_serverEndPoint == null)
            {
                Log.Logger.Information("Не создан экземпляр сервера");
                return;
            }

            _serverSocket.Bind(_serverEndPoint);
            _serverSocket.Listen(100);

            _isListen = true;

            Log.Logger.Information("Ожидание новых подключений...");

            _acceptConnectionsTokenSource = new CancellationTokenSource();
            _acceptConnectionsToken = _acceptConnectionsTokenSource.Token;

            _acceptConnections = Task.Run(AcceptConnectionsAsync, _acceptConnectionsToken);
        }
        catch (Exception ex)
        {
            Log.Logger.Information($"Произошла ошибка при включении сервера: {ex}");
        }
    }

    private async Task AcceptConnectionsAsync()
    {
        while (!_acceptConnectionsToken.IsCancellationRequested)
        {
            var socketClient = new SocketClient();
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(socketClient.Token, _acceptConnectionsToken);

            try
            {
                var newClientSocket = await _serverSocket.AcceptAsync(_acceptConnectionsToken);

                var clientIp = newClientSocket.RemoteEndPoint?.ToString();

                Log.Logger.Information($"Подключился новый клиент: {clientIp}");

                await socketClient.CreateAsync(newClientSocket, _acceptConnectionsToken);

                _clientController.AddClient(clientIp, socketClient);

                _events.CallClientConnectedEvent(this, clientIp);

                // await _clientController.BroadcastAllAsync("Подключился новый клиент", _listenerToken);

                var _ = Task.Run(() => DataReceiverAsync(socketClient, _token), linkedCts.Token);
            }
            catch (Exception ex)
            {
                if (ex is TaskCanceledException ||
                    ex is OperationCanceledException ||
                    ex is ObjectDisposedException ||
                    ex is InvalidOperationException)
                {
                    _isListen = false;
                    // if (socketClient != null)
                    //     socketClient.Dispose();

                    Log.Logger.Information($"Stopped listening");
                    break;
                }
                else
                {
                    // if (socketClient != null)
                    //     socketClient.Dispose();

                    Log.Logger.Error($"Возникла ошибка во время ожидация новых подключений: {ex}");
                    continue;
                }
            }
        }

        _isListen = false;
    }

    private async Task DataReceiverAsync(SocketClient socketClient, CancellationToken token)
    {
        var ip = socketClient.Ip;
        
        Log.Logger.Information($"Ожидание сообщений от {ip}");

        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, socketClient.Token);

        while (true)
        {
            try
            {
                if (!_clientController.IsExist(ip))
                {
                    Log.Logger.Information($"Клиент {ip} отключился");
                    break;
                }

                if (socketClient.Token.IsCancellationRequested)
                {
                    Log.Logger.Information($"Клиент {ip} отменил получение сообщений");
                    break;
                }

                var data = await ReadDataAsync(socketClient, linkedCts.Token);
                if (data == null)
                {
                    await Task.Delay(10, linkedCts.Token);
                    continue;
                }

                _events.CallDataReceiveEvent(this, data, ip);
            }
            catch (Exception e)
            {
                if(e is IOException || e is SocketException || e is TaskCanceledException || e is ObjectDisposedException)
                    Log.Logger.Error($"s");
                else
                    Log.Logger.Error($"data receiver exception [{ip}]:{Environment.NewLine}{e}{Environment.NewLine}");
                break;
            }
        }

        _events.CallClientDisconnectedEvent(this);

        _clientController.RemoveClient(ip);

        // if (socketClient != null) 
        //     socketClient.Dispose();
    }

    private async Task<byte[]> ReadDataAsync(SocketClient socketClient, CancellationToken token)
    {
        var buffer = new byte[1024];
        var read = 0;

        await using var ms = new MemoryStream();
        while (true)
        {
            read = await socketClient.ReceiveAsync(buffer, SocketFlags.None, token);

            if (read <= 0)
                throw new SocketException();

            await ms.WriteAsync(buffer, 0, read, token);
            return ms.ToArray();
        }
    }
}