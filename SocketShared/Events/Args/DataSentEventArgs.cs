namespace SocketShared.Events.Args;

/// <summary>
/// Событие отправки данных
/// </summary>
public class DataSentEventArgs : EventArgs
{
    /// <summary>
    /// Массив байтов отправляемых данных
    /// </summary>
    public byte[] Data { get; }

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="data">Массив байтов отправляемых данных</param>
    public DataSentEventArgs(byte[] data)
    {
        Data = data;
    }
}