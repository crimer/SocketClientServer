using Newtonsoft.Json;
using System.Text;

namespace SocketServer.Sockets.Helpers
{
    /// <summary>
    /// Класс вспомогательных функций для сетвера сокетов
    /// </summary>
    public class SocketHelper
    {
        /// <summary>
        /// Данные в строку json, потом в byte[]
        /// </summary>
        /// <param name="data">Данные</param>
        /// <returns>Байты</returns>
        public static byte[] SerializeData(object data)
        {
            var str = JsonConvert.SerializeObject(data);
            return Encoding.Default.GetBytes(str);
        }

        /// <summary>
        /// Байты в строку json, потом в модель
        /// </summary>
        /// <typeparam name="T">Тип модели</typeparam>
        /// <param name="bytes">Данные</param>
        /// <returns>Модель</returns>
        public static T DeserializeData<T>(byte[] bytes)
        {
            var srt = Encoding.Default.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(srt);
        }
    }
}
