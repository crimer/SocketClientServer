namespace SocketServer.Sockets.Dto
{
    public class SocketResponseMessage
    {
        public string Message { get; private set; }

        protected SocketResponseMessage(string message)
        {
            Message = message;
        }

        public static SocketResponseMessage Ok(string message)
            => new SocketResponseMessage(message);
    }

    public class SocketResponseMessage<T> : SocketResponseMessage
    {
        public T Data { get; private set; }
        protected SocketResponseMessage(T data, string message) : base(message)
        {
            Data = data;
        }

        public static SocketResponseMessage<T> Ok(T data, string message = null)
            => new SocketResponseMessage<T>(data, message);
    }
}
