using System;
using System.Collections.Concurrent;
using System.Linq;

namespace SocketServer.Sockets
{
    /// <summary>
    /// Менеджер подключений сокетов
    /// </summary>
    public class SocketConnectionManager
    {
        private ConcurrentDictionary<Guid, SocketClient> _sockets = new ConcurrentDictionary<Guid, SocketClient>();

        /// <summary>
        /// ПОлучение клиента по Guid
        /// </summary>
        /// <param name="id">Guid клиента</param>
        /// <returns>Клиент</returns>
        public SocketClient GetSocketById(Guid id) => _sockets.FirstOrDefault(p => p.Key == id).Value;

        /// <summary>
        /// Получение Guid клиента по клиенту 
        /// </summary>
        /// <param name="socket">Клиент</param>
        /// <returns>Guid клиента</returns>
        public Guid GetId(SocketClient socket) => _sockets.FirstOrDefault(p => p.Value == socket).Key;

        /// <summary>
        /// Получение всех подключенных клиентов 
        /// </summary>
        /// <returns>Словать подключенных клиентов</returns>
        public ConcurrentDictionary<Guid, SocketClient> GetAll() => _sockets;

        /// <summary>
        /// Добавление клиента в менеджер
        /// </summary>
        /// <param name="socket">Клиент</param>
        public void AddSocket(SocketClient socket) => _sockets.TryAdd(socket.Id, socket);

        /// <summary>
        /// Удаление клиента из менеджера
        /// </summary>
        /// <param name="id">Guid клиента</param>
        public void RemoveSocket(Guid id) => _sockets.TryRemove(id, out SocketClient socket);
    }
}