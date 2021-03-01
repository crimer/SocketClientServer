using Serilog;
using SocketServer.Sockets.Dto;
using SocketServer.Sockets.EventArguments;
using System;
using System.Diagnostics;
using System.Net;
using SocketServer.Sockets;
using Serilog.Events;
using System.IO;

namespace SocketServer
{
    class Program
    {
        private const string IPADDRESS = "127.0.0.1";
        private const int PORT = 333;
        public static Sockets.SocketServer socketServer;
        static Random random = new Random();
        static string[] _r = { "image1.png", "image2.png", "image3.png", "image4.png", "image5.png", "image21.png", "image22.png", "image23.png", "image24.png", "image25.png" };
        /// <summary>
        /// Константы команд 
        /// </summary>
        private const string START_COMMAND = "START_COMMAND";
        private const string STOP_COMMAND = "STOP_COMMAND";

        static void Main(string[] args)
        {
            SetUpLogger();

            socketServer = new Sockets.SocketServer(new IPEndPoint(IPAddress.Parse(IPADDRESS), PORT));
            socketServer.OnMessageReceived += SocketReceivedMessage;

            // Закрыть приложение только тогда когда нажмут кнопку закрыть
            Process.GetCurrentProcess().WaitForExit();
        }

        /// <summary>
        /// Настройка логирования
        /// </summary>
        private static void SetUpLogger()
        {
            string appDirectory = Environment.CurrentDirectory;
            string pathToLogsDir = Path.Combine(appDirectory, "logs_socket_server");
            var logsExists = Directory.Exists(pathToLogsDir);
            
            if (!logsExists)
                Directory.CreateDirectory(pathToLogsDir);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(
                    shared: true,
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: LogEventLevel.Error,
                    retainedFileCountLimit: 10,
                    path: pathToLogsDir + "/log-.txt")
                .CreateLogger();
        }

        private static void SocketReceivedMessage(object sender, OnMessageReceivedEventArgs e)
        {
            switch (e.RawMessage)
            {
                case START_COMMAND:
                    e.SocketClient.StartPeriodicTask(() =>
                    {
                        int idx = random.Next(0, _r.Length-1);
                        socketServer.SendMessage(e.SocketClient, SocketResponseMessage.Ok($"Hi {_r[idx]}"));
                    });
                    Log.Information("Start PeriodicTask");
                    break;
                case STOP_COMMAND:
                    e.SocketClient.StopPeriodicTask();
                    Log.Information("Stop PeriodicTask");
                    break;
                default:
                    break;
            }
        }
    }
}