namespace SocketServer.Sockets.EventArguments
{
    /// <summary>
    /// Аргументы события отключения клиента
    /// </summary>
    public class OnClientDisconnectedEventArgs
    {
        private SocketClient _socketClient;
        public SocketClient SocketClient { get => _socketClient; }
        public OnClientDisconnectedEventArgs(SocketClient socketClient)
        {
            _socketClient = socketClient;
        }
    }
}
