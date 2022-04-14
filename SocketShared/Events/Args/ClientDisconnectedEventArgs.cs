namespace SocketShared.Events.Args;

/// <summary>
/// Событие отключения клиента
/// </summary>
public class ClientDisconnectedEventArgs : EventArgs
{
    /// <summary>
    /// IP клиента
    /// </summary>
    public string ClientIp { get; }

    
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="clientIp">IP клиента</param>
    public ClientDisconnectedEventArgs(string clientIp)
    {
        ClientIp = clientIp;
    }
}