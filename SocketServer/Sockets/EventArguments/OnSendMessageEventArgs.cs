using SocketServer.Sockets.Dto;

namespace SocketServer.Sockets.EventArguments
{
    public class OnSendMessageEventArgs
    {
        private SocketClient _socketClient;
        public SocketClient SocketClient { get => _socketClient; }

        private SocketResponseMessage _message;
        public SocketResponseMessage Message { get => _message; }
        public OnSendMessageEventArgs(SocketClient socketClient, SocketResponseMessage message)
        {
            _socketClient = socketClient;
            _message = message;
        }
    }
}
