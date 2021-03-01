using System;

namespace SocketServer.Sockets.EventArguments
{
    public class OnClientConnectedEventArgs : EventArgs
    {
        private SocketClient _socketClient;
        public SocketClient SocketClient { get => _socketClient; }
        public OnClientConnectedEventArgs(SocketClient socketClient)
        {
            _socketClient = socketClient;
        }
    }
}
