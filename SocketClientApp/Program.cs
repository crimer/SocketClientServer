using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketClientApp
{
    internal class Program
    {
        private static Socket _socket;
        private static byte[] _bytes = new byte[1024];
        private static string ivSocket = "77.35.66.66"; 
        private static int ivPort = 30050; 

        public static void Main(string[] args)
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 333);
            _socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            LoopConnect(remoteEP);
            SendLoop();
            Console.ReadLine();
        }

        private static void SendLoop()
        {
            while (true)
            {
                try
                {
                    Console.Write("Enter message: ");
                    string res = Console.ReadLine();
                    
                    byte[] msg = Encoding.ASCII.GetBytes(res);

                    int bytesSent = _socket.Send(msg);

                    _socket.BeginReceive(_bytes, 0, _bytes.Length, SocketFlags.None, ReceiveCallback, null);
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }
        }

        private static void ReceiveCallback(IAsyncResult asyncResult)
        {
            // проверить на наличие подключения к серверу, а то exception
            int received = _socket.EndReceive(asyncResult);

            byte[] messageBuffer = new byte[received];
                
            Array.Copy(_bytes, messageBuffer, received);
                
            var str = Encoding.Default.GetString(messageBuffer);
                
            Console.WriteLine($"From server: {str}");
        }

        public static void LoopConnect(IPEndPoint remoteEP)
        {
            int countConnect = 0;
            while (!_socket.Connected)
            {
                try
                {
                    countConnect++;
                    _socket.Connect(remoteEP);
                    Console.WriteLine($"Connected!");
                }
                catch (SocketException e)
                {
                    Console.Clear();
                    Console.WriteLine($"Connection try: {countConnect}");
                }
            }
        }
    }
}