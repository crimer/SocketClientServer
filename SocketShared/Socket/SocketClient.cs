using System.Net;
using System.Net.Sockets;
using System.Text;
using Serilog;
using SocketShared.Events;

namespace SocketShared.Socket;

public class SocketClient : IDisposable
{
    public string Ip => _ip;
    public bool IsConnected { get; private set; }

    public string ServerIp => $"{_serverIp}:{_serverPort}";

    public IPEndPoint LocalIp => _clientSocket == null || !IsConnected ? null : (IPEndPoint)_clientSocket.LocalEndPoint;
    public CancellationToken Token { get; set; }
    public ClientSocketEvents Events => _clientSocketEvents;


    private System.Net.Sockets.Socket _clientSocket;
    private string _ip;
    private ClientSocketEvents _clientSocketEvents;
    private string _serverIp = null;
    private int _serverPort = 0;
    private Task _dataReceiver = null;

    public async Task CreateAsync(System.Net.Sockets.Socket socket, CancellationToken token)
    {
        _clientSocket = socket;

        _ip = socket.RemoteEndPoint.ToString();

        _clientSocketEvents = new ClientSocketEvents();
    }

    public async Task TryConnectAsync(string ip, int port, CancellationToken token)
    {
        if (IsConnected)
            throw new InvalidOperationException("Клиент уже подключен");

        if (!IPAddress.TryParse(ip, out var ipAddress))
            throw new InvalidOperationException("Не валидный IP адрес");

        _clientSocket = new System.Net.Sockets.Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        while (!_clientSocket.Connected)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), token);
            try
            {
                await _clientSocket.ConnectAsync(ipAddress, port, token);
                this.Events.CallClientConnectedEvent(this, ipAddress);
            }
            catch
            {
            }
        }

        var _ = Task.Run(() => DataReceiverAsync(socketClient, _token), linkedCts.Token);


        Log.Logger.Information("Подключились к серверу");
    }

    /// <summary>
    /// Отключение от сервера
    /// </summary>
    public void Disconnect()
    {
        if (!IsConnected)
        {
            Log.Logger.Information($"Клиент уже отключен");
            return;
        }

        Log.Logger.Information($"Отключение от сервера {this.ServerIp}");

        _tokenSource.Cancel();
        _dataReceiver.Wait();
        _client.Close();
        this.IsConnected = false;
    }

    /// <summary>
    /// Send data to the server.
    /// </summary>
    /// <param name="data">String containing data to send.</param>
    /// <param name="token"></param>
    public Task SendAsync(string data, CancellationToken token = default)
    {
        if (string.IsNullOrEmpty(data))
            throw new ArgumentNullException(nameof(data));

        if (!this.IsConnected)
            throw new IOException("Нет подключения к серверу");

        var bytes = Encoding.UTF8.GetBytes(data);
        return this.SendAsync(bytes, token);
    }

    /// <summary>
    /// Send data to the server.
    /// </summary> 
    /// <param name="data">Byte array containing data to send.</param>
    public Task SendAsync(byte[] data, CancellationToken token = default)
    {
        if (data == null || data.Length < 1)
            throw new ArgumentNullException(nameof(data));

        if (!this.IsConnected)
            throw new IOException("Not connected to the server; use Connect() first.");

        if (token == default)
            token = _token;

        using var ms = new MemoryStream();
        ms.Write(data, 0, data.Length);
        ms.Seek(0, SeekOrigin.Begin);
        SendInternal(data.Length, ms);
    }


    private async Task DataReceiver(CancellationToken token)
    {
        while (!token.IsCancellationRequested && this._clientSocket is { Connected: true })
        {
            try
            {
                await DataReadAsync(token).ContinueWith(async task =>
                {
                    if (task.IsCanceled)
                        return null;

                    var data = task.Result;

                    if (data != null)
                    {
                        this.Events.CallDataReceiveEvent(this, data, Ip);
                        return data;
                    }
                    else
                    {
                        await Task.Delay(100, token);
                        return null;
                    }
                }, token).ContinueWith(task => { }, token).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Log.Logger.Error($"Ошабка во время получения данных: {e}");
                break;
            }
        }

        Log.Logger.Information($"Отключение клиента");

        this.IsConnected = false;

        this.Events.CallClientDisconnectedEvent(this, Ip);
        
        Dispose();
    }

    private async Task<byte[]> DataReadAsync(CancellationToken token)
    {
        var buffer = new byte[1024];
        var read = 0;

        try
        {
            read = await _networkStream.ReadAsync(buffer, 0, buffer.Length, token).ConfigureAwait(false);


            if (read > 0)
            {
                await using var ms = new MemoryStream();
                ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
            else
            {
                throw new SocketException();
            }
        }
        catch (IOException)
        {
            return null;
        }
    }

    public void Dispose()
    {
        // if (disposing)
        // {
        //     _isConnected = false;
        //
        //     if (_tokenSource != null)
        //     {
        //         if (!_tokenSource.IsCancellationRequested)
        //         {
        //             _tokenSource.Cancel();
        //             _tokenSource.Dispose();
        //         }
        //     }
        //
        //     if (_networkStream != null)
        //     {
        //         _networkStream.Close();
        //         _networkStream.Dispose(); 
        //     }
        //
        //     if (_client != null)
        //     {
        //         _client.Close();
        //         _client.Dispose(); 
        //     }
        //
        //     Logger?.Invoke($"{_header}dispose complete");
        // }
        _clientSocket.Dispose();
    }
}