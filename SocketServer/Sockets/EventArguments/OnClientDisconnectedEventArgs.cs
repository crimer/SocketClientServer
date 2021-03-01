namespace SocketServer.Sockets.EventArguments
{
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
