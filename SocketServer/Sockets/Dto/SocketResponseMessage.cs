using Newtonsoft.Json;

namespace SocketServer.Sockets.Dto
{
    /// <summary>
    /// Модель ответа от сокет сервера
    /// </summary>
    public class SocketResponseMessage
    {
        /// <summary>
        /// Сообщение
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="message">Сообщение</param>
        protected SocketResponseMessage(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Ф-ия создания ответа
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <returns>Модель ответа</returns>
        public static SocketResponseMessage Ok(string message)
            => new SocketResponseMessage(message);
    }

    /// <summary>
    /// Модель ответа от сокет сервера
    /// </summary>
    /// <typeparam name="T">Тип отправляемых данных</typeparam>
    public class SocketResponseMessage<T> : SocketResponseMessage
    {
        /// <summary>
        /// Отправляемые данные
        /// </summary>
        [JsonProperty("data")]
        public T Data { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="data">Данные</param>
        /// <param name="message">Сообщение</param>
        protected SocketResponseMessage(T data, string message) : base(message)
        {
            Data = data;
        }

        /// <summary>
        /// Ф-ия создания ответа
        /// </summary>
        /// <param name="message">Данные, сообщение</param>
        /// <returns>Модель ответа</returns>
        public static SocketResponseMessage<T> Ok(T data, string message = null)
            => new SocketResponseMessage<T>(data, message);
    }
}
