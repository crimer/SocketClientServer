using System.Collections.Concurrent;

namespace SocketShared.Socket;

public class ClientController
{
    private ConcurrentDictionary<string, SocketClient> Clients { get; }

    public ClientController()
    {
        Clients = new ConcurrentDictionary<string, SocketClient>();
    }

    public async Task BroadcastAllAsync(string message, CancellationToken token)
    {
        foreach (var socketClient in Clients.Values)
            await socketClient.SendAsync(message, token);
    }

    public void DisconnectAllClients()
    {
        foreach (var client in Clients)
        {
            client.Value.Disconnect();
        }
        Clients.Clear();
    }
    
    public void AddClient(string ip, SocketClient socketClient) => Clients.TryAdd(ip, socketClient);
    
    public void RemoveClient(string ip) => Clients.TryRemove(ip, out _);

    public SocketClient GetSocketById(string ip) => Clients.FirstOrDefault(p => p.Key == ip).Value;
    public bool IsExist(string ip) => Clients.ContainsKey(ip);
}