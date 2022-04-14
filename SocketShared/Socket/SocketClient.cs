using System.Net;
using System.Net.Sockets;
using System.Text;
using Serilog;

namespace SocketShared.Socket;

public class SocketClient
{
    public string Ip => _ip;
    public ClientReceiver Receiver { get; private set; }
    public CancellationToken Token { get; set; }

    private System.Net.Sockets.Socket _clientSocket; 
    private string _ip; 
    
    public async Task CreateAsync(System.Net.Sockets.Socket socket, CancellationToken token)
    {
        _clientSocket = socket;
        
        _ip = socket.RemoteEndPoint.ToString();
        
        Receiver = new ClientReceiver(_clientSocket, _ip);
        // await Receiver.StartReceivingAsync(token);
    }
    
    public async Task TryConnectAsync(string ip, int port, CancellationToken token)
    {
        if (!IPAddress.TryParse(ip, out var ipAddress))
            throw new InvalidOperationException("Не валидный IP адрес");
        
        _clientSocket = new System.Net.Sockets.Socket(
            ipAddress.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);
        
        while (!_clientSocket.Connected)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), token);

            try
            {
                await _clientSocket.ConnectAsync(ipAddress, port, token);
            }
            catch { }
        }
        Log.Logger.Information("Подключились к серверу");
        
        // Id = Guid.NewGuid();
        // Receiver = new ClientReceiver(ClientSocket, Id);
        //
        // await Receiver.StartReceivingAsync(token);
    }

    public async Task SendAsync(string message, CancellationToken token)
    {
        try
        {
            var fullPacket = new List<byte>();
            fullPacket.AddRange(BitConverter.GetBytes(message.Length));
            fullPacket.AddRange(Encoding.Default.GetBytes(message));
            
            var body = new ReadOnlyMemory<byte>(fullPacket.ToArray());
            await _clientSocket.SendAsync(body, SocketFlags.None, token);
        }
        catch (Exception ex)
        {
            throw new Exception();
        }
    }

    public void Disconnect()
    {
        throw new NotImplementedException();
    }

    public ValueTask<int> ReceiveAsync(byte[] buffer, SocketFlags flag, CancellationToken token) => this._clientSocket.ReceiveAsync(buffer, flag, token);
}