using Serilog;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketServer.Sockets
{
    /// <summary>
    /// Класс клиента сервера сокетов
    /// </summary>
    public class SocketClient
    {
        /// <summary>
        /// Поля
        /// </summary>
        private Socket _socket;
        private SocketServer _serverSocket;
        private Guid _id;
        private byte[] _buffer = new byte[1024];
        private IPEndPoint _clientIpEndPoint;
        private System.Timers.Timer _timer;
        private bool _isPeriodicTaskWork = false;


        /// <summary>
        /// Свойства
        /// </summary>
        public Socket Socket => _socket;
        public SocketServer SocketServer => _serverSocket;
        public Guid Id  => _id;
        public bool IsConnected => _socket.Connected;
        public IPEndPoint ClientIpEndPoint => _clientIpEndPoint;
        public string FullAddress => $"{ClientIpEndPoint.Address}:{ClientIpEndPoint.Port}";

        /// <summary>
        /// Конструктор сокет клиента
        /// </summary>
        /// <param name="serverSocket">Сервер к которому подключился</param>
        /// <param name="clientSocket">Сокет клиента</param>
        public SocketClient(SocketServer serverSocket, Socket clientSocket)
        {
            _socket = clientSocket;
            _serverSocket = serverSocket;
            _id = Guid.NewGuid();
            _clientIpEndPoint = (IPEndPoint)clientSocket.RemoteEndPoint;

            _timer = new System.Timers.Timer();
            _timer.Stop();

            _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, null);
        }

        /// <summary>
        /// Ф-ия обработчик нового сообщения
        /// </summary>
        /// <param name="asyncResult"></param>
        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            try
            {
                if (!IsConnected)
                    throw new SocketException(10061);

                int bytesReceived = _socket.EndReceive(asyncResult);

                byte[] messageBuffer = new byte[bytesReceived];
                if (bytesReceived > 0)
                {
                    Array.Copy(_buffer, messageBuffer, bytesReceived);
                
                    var str = Encoding.Default.GetString(messageBuffer);
                
                    _serverSocket.ReceiveMessage(this, str);

                    _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, null);
                }
            }
            catch(SocketException socketEx)
            {
                if(socketEx.SocketErrorCode == SocketError.ConnectionRefused || 
                    socketEx.SocketErrorCode == SocketError.ConnectionReset)
                {
                    Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"-- Error - MessageCallback {ex}");
            }
        }

        /// <summary>
        /// Запуск задачи каждый заданый интервал
        /// </summary>
        /// <param name="action">Действие</param>
        /// <param name="interval">Интервал (мс)</param>
        public void StartPeriodicTask(Action action, int interval = 2000)
        {
            try
            {
                if (_isPeriodicTaskWork)
                    return;

                _timer.Interval = interval;
                _timer.Elapsed += delegate
                {
                    action();
                };
                _timer.Start();
                _isPeriodicTaskWork = true;
                Log.Information($"{this.FullAddress} (client) Start PeriodicTask");

            }
            catch(Exception ex)
            {
                Log.Error($"-- Error - StartPeriodicTask {ex}");
            }
        }

        /// <summary>
        /// Остановка задачи каждый заданый интервал
        /// </summary>
        /// <param name="action">Действие</param>
        /// <param name="interval">Интервал (мс)</param>
        public void StopPeriodicTask()
        {
            try
            {
                if (!_isPeriodicTaskWork)
                    return;

                _timer.Stop();
                _isPeriodicTaskWork = false;
                Log.Information($"{this.FullAddress} (client) Stop PeriodicTask");
            }
            catch (Exception ex)
            {
                Log.Error($"-- Error - StopPeriodicTask {ex}");
            }
        }

        /// <summary>
        /// Отправить сообщение клиенту
        /// </summary>
        /// <param name="bytes">Данные</param>
        public void Send(byte[] bytes)
        {
            try
            {
                _socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, SendCallback, null);
            }
            catch (Exception ex)
            {
                Log.Error($"Client Send: {ex}");
            }
        }
        
        /// <summary>
        /// Ф-ия обработчик отправки сообщения
        /// </summary>
        /// <param name="asyncResult"></param>
        private void SendCallback(IAsyncResult asyncResult)
        {
            _socket.EndSend(asyncResult);
        }

        /// <summary>
        /// Отключить клиента
        /// </summary>
        /// <param name="reuseSocket">Позволять повторно использовать сокет</param>
        public void Disconnect(bool reuseSocket)
        {
            try
            {
                if (_isPeriodicTaskWork)
                {
                    _timer.Stop();
                    _timer.Close();
                    _timer.Dispose();
                }

                if (IsConnected)
                    _socket.Disconnect(reuseSocket);

                _socket.Close();
                _socket.Dispose();

                _serverSocket.ClientDisconnect(this);
            }
            catch (Exception ex)
            {
                Log.Error($"Client Disconnect: {ex}");
            }
        }
    }
}
