using System.Net.Sockets;
using System.Text;

namespace SocketShared.Socket;

public class ClientReceiver
{
    private byte[] _buffer;
    private readonly System.Net.Sockets.Socket _receiveSocket;
    private readonly string _ip;

    public ClientReceiver(System.Net.Sockets.Socket receiveSocket, string ip)
    {
        _receiveSocket = receiveSocket;
        _ip = ip;
        _buffer = new byte[4];
    }
    
    // public async Task StartReceivingAsync(CancellationToken cts)
    // {
    //     try
    //     {
    //         var res = await _receiveSocket.ReceiveAsync(_buffer, SocketFlags.None, cts);
    //         if (res > 1)
    //         {
    //             _buffer = new byte[BitConverter.ToInt32(_buffer, 0)];
    //             _receiveSocket.Receive(_buffer, _buffer.Length, SocketFlags.None);
    //             var data = Encoding.Default.GetString(_buffer);
    //         }
    //         else
    //         {
    //             await DisconnectAsync();
    //         }
    //         // _receiveSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, null);
    //     }
    //     catch
    //     {
    //         if (!_receiveSocket.Connected)
    //             await DisconnectAsync();
    //         // else
    //         //     StartReceiving();
    //     }
    // }
    //
    // private async Task DisconnectAsync()
    // {
    //     await _receiveSocket.DisconnectAsync(true);
    //     ClientController.RemoveClient(_ip);
    // }
}