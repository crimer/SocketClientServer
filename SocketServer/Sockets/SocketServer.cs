using Serilog;
using SocketServer.Sockets.Dto;
using SocketServer.Sockets.EventArguments;
using SocketServer.Sockets.Helpers;
using System;
using System.Net;
using System.Net.Sockets;

namespace SocketServer.Sockets
{
    /// <summary>
    /// Класс сервера для сокетов
    /// </summary>
    public class SocketServer
    {
        /// <summary>
        /// События
        /// </summary>
        public event EventHandler<OnClientConnectedEventArgs> OnClientConnect;
        public event EventHandler<OnClientDisconnectedEventArgs> OnClientDisconnected;
        public event EventHandler<OnMessageReceivedEventArgs> OnMessageReceived;
        public event EventHandler<OnSendMessageEventArgs> OnSendMessage;

        /// <summary>
        /// Поля
        /// </summary>
        private readonly IPEndPoint _ipEndPoint;
        private readonly Socket _socket;
        private SocketConnectionManager _connectionManager;
        private bool _isListening = false;

        /// <summary>
        /// Свойства
        /// </summary>
        public IPEndPoint IpEndPoint { get => _ipEndPoint; }
        public Socket Socket { get => _socket; }
        public SocketConnectionManager ConnectionManager => _connectionManager;
        public string FullAddress => $"{_ipEndPoint.Address}:{_ipEndPoint.Port}";

        /// <summary>
        /// Конструктор сокет сервера
        /// </summary>
        /// <param name="ipEndPoint">Ip точка</param>
        public SocketServer(IPEndPoint ipEndPoint)
        {
            if (ipEndPoint == null)
                return;
            
            _connectionManager = new SocketConnectionManager();
            _ipEndPoint = ipEndPoint;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Console.WriteLine($"------ WebSocket Server Started on {FullAddress} ------");

            Start();
        }

        /// <summary>
        /// Запуск прослушивания сервера
        /// </summary>
        private void Start()
        {
            try
            {
                if (_isListening)
                {
                    Log.Information("Socket Server already running");
                    return;
                }

                _isListening = true;
                _socket.Bind(_ipEndPoint);
                _socket.Listen(50);
                _socket.BeginAccept(ConnectionCallback, null);
                
                Log.Information("Server started");
            }
            catch (Exception ex)
            {
                Log.Error($"StartServer: {ex}");
            }
        }

        /// <summary>
        /// Завершение прослушивание сервера
        /// </summary>
        private void Stop()
        {
            if(!_isListening)
            {
                Log.Warning("Socket Server already is not running");
                return;
            }    
            _socket.Close();
            _socket.Dispose();
            Log.Information("Server stopepd");
        }

        /// <summary>
        /// Ф-ия обработчик, вызывается когда соткет пытается получить входящее подключение
        /// </summary>
        /// <param name="asyncResult">Асинхронный контекст</param>
        private void ConnectionCallback(IAsyncResult asyncResult)
        {
            try
            {
                Socket clientSocket = _socket.EndAccept(asyncResult);
                
                SocketClient client = new SocketClient(this, clientSocket);
                
                _connectionManager.AddSocket(client);
                
                Log.Information($"Connected new client - {client.FullAddress}");
                
                OnClientConnect?.Invoke(this, new OnClientConnectedEventArgs(client));
                
                _socket.BeginAccept(ConnectionCallback, null);
            }
            catch (Exception ex)
            {
                Log.Error($"ConnectionCallback: {ex}");
            }
        }

        /// <summary>
        /// Ф-ия отправки сообщения сокету клиента
        /// </summary>
        /// <param name="client">Сокет клиента</param>
        /// <param name="response">Ответ</param>
        public void SendMessage(SocketClient client, SocketResponseMessage response)
        {
            try
            {
                var bytes = SocketHelper.SerializeData(response);

                client.Send(bytes);

                Log.Information($"From {this.FullAddress} (server) to {client.FullAddress} (client): {response.Message}");

                OnSendMessage?.Invoke(this, new OnSendMessageEventArgs(client, response));
            }
            catch(ObjectDisposedException ex)
            {
                Log.Warning($"Client {client.FullAddress} refused connection {ex}");
                client.Disconnect(false);
            }
            catch (Exception ex)
            {
                Log.Error($"SendMessage: {ex}");
            }
        }
        
        /// <summary>
        /// Ф-ия отправки сообщения сокету клиента по Guid клиента
        /// </summary>
        /// <param name="clientId">Guid сокета клиента</param>
        /// <param name="response">Ответ</param>
        public void SendMessage(Guid clientId, SocketResponseMessage response)
        {
            try
            {
                var bytes = SocketHelper.SerializeData(response);
                
                var client = _connectionManager.GetSocketById(clientId);
                
                client.Send(bytes);
                
                Log.Information($"From {this.FullAddress} (server) to {client.FullAddress} (client): {response.Message}");

                OnSendMessage?.Invoke(this, new OnSendMessageEventArgs(client, response));
            }
            catch (Exception ex)
            {
                Log.Error($"SendMessage by Guid: {ex}");
            }
        }

        /// <summary>
        /// Ф-ия обработчик, вызывается когда соткету приходит новое сообщение
        /// </summary>
        /// <param name="client">Сокет отправителя сообщения</param>
        /// <param name="message">Сообщение</param>
        public void ReceiveMessage(SocketClient client, string message)
        {
            try
            {
                Log.Information($"Receive from {client.FullAddress} (client): {message}");
                
                OnMessageReceived?.Invoke(this, new OnMessageReceivedEventArgs(client, message));
            }
            catch (Exception ex)
            {
                Log.Error($"ReceiveMessage: {ex}");
            }
        }

        /// <summary>
        /// Ф-ия вызывается когда отключается сокет клиента
        /// </summary>
        /// <param name="client">Сокет клиента</param>
        public void ClientDisconnect(SocketClient client)
        {
            try
            {
                var socketId = _connectionManager.GetId(client);
                _connectionManager.RemoveSocket(socketId);

                Log.Information($"Client {client.FullAddress} disconnected");
                
                OnClientDisconnected?.Invoke(this, new OnClientDisconnectedEventArgs(client));
            }
            catch (Exception ex)
            {
                Log.Error($"ClientDisconnect: {ex}");
            }
        }
    }
}