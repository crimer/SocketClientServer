namespace SocketShared.Events.Args;

/// <summary>
/// Событие постепления новых данных
/// </summary>
public class DataReceiveEventArgs : EventArgs
{
    /// <summary>
    /// IP клиента
    /// </summary>
    public string Ip { get; }
    
    /// <summary>
    /// Массив байтов новых полученных данных
    /// </summary>
    public byte[] Data { get; }

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="ip">IP клиента</param>
    /// <param name="data">Массив байтов новых полученных данных</param>
    public DataReceiveEventArgs(string ip, byte[] data)
    {
        Ip = ip;
        Data = data;
    }
}