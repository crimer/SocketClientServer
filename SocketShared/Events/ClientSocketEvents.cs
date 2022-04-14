using SocketShared.Events.Args;

namespace SocketShared.Events;

/// <summary>
/// События сокет сервера
/// </summary>
public class ClientSocketEvents
{
    /// <summary>
    /// Событие подключения клиента
    /// </summary>
    public event EventHandler<ClientConnectedEventArgs>? Connected;
    
    /// <summary>
    /// Событие отключения клиента
    /// </summary>
    public event EventHandler<ClientDisconnectedEventArgs>? Disconnected;
    
    /// <summary>
    /// Событие поступления новых данных
    /// </summary>
    public event EventHandler<DataReceiveEventArgs>? DataReceive;
    
    /// <summary>
    /// Событие отправки данных
    /// </summary>
    public event EventHandler<DataSentEventArgs>? DataSent;

    /// <summary>
    /// Вызов события подключения клиента
    /// </summary>
    /// <param name="sender">Кто вызвал событие</param>
    /// <param name="clientIp">IP клиента</param>
    public void CallClientConnectedEvent(object? sender, string clientIp) => this.Connected?.Invoke(sender, new ClientConnectedEventArgs(clientIp));
    
    /// <summary>
    /// Вызов события отключения клиента
    /// </summary>
    /// <param name="sender">Кто вызвал событие</param>
    /// <param name="clientIp">IP клиента</param>
    public void CallClientDisconnectedEvent(object? sender, string clientIp) => this.Disconnected?.Invoke(sender, new ClientDisconnectedEventArgs(clientIp));
    
    /// <summary>
    /// Вызов события поступления новых данных
    /// </summary>
    /// <param name="sender">Кто вызвал событие</param>
    /// <param name="data">Массив байтов новых полученных данных</param>
    /// <param name="clientIp">IP клиента</param>
    public void CallDataReceiveEvent(object? sender, byte[] data, string clientIp) => this.DataReceive?.Invoke(sender, new DataReceiveEventArgs(clientIp, data));
    
    /// <summary>
    /// Вызов события отправки данных
    /// </summary>
    /// <param name="sender">Кто вызвал событие</param>
    /// <param name="data">Массив байтов отправляемых данных</param>
    public void CallDataSentEvent(object? sender, byte[] data) => this.DataSent?.Invoke(sender, new DataSentEventArgs(data));
}