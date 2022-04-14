namespace SocketShared.Events.Args;

/// <summary>
/// Событие подключения нового клиента
/// </summary>
public class ClientConnectedEventArgs : EventArgs
{
    /// <summary>
    /// IP клиента
    /// </summary>
    public string ClientIp { get; }

    /// <summary>
    /// Конструктор 
    /// </summary>
    /// <param name="clientIp">IP клиента</param>
    public ClientConnectedEventArgs(string clientIp)
    {
        ClientIp = clientIp;
    }
}