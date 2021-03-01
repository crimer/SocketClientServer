using System;

namespace SocketServer.Sockets.EventArguments
{
    public class OnMessageReceivedEventArgs : EventArgs
    {
        private SocketClient _socketClient;
        public SocketClient SocketClient { get => _socketClient; }
        
        private string _rawMessage;
        public string RawMessage { get => _rawMessage; }
        public OnMessageReceivedEventArgs(SocketClient socketClient, string rawMessage)
        {
            _socketClient = socketClient;
            _rawMessage = rawMessage;
        }
    }
}
