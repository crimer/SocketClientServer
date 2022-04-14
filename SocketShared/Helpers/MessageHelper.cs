using System.Text;
using Newtonsoft.Json;

namespace SocketShared.Helpers;

/// <summary>
/// Клас помошник для формирования сообщений
/// </summary>
public class MessageHelper
{
    /// <summary>
    /// Данные в строку JSON, потом в byte[]
    /// </summary>
    /// <param name="data">Данные</param>
    /// <returns>Байты</returns>
    public static byte[] SerializeData(object data)
    {
        var str = JsonConvert.SerializeObject(data);
        return Encoding.Default.GetBytes(str);
    }

    /// <summary>
    /// Байты в строку JSON, потом в модель
    /// </summary>
    /// <typeparam name="T">Тип модели</typeparam>
    /// <param name="bytes">Данные</param>
    /// <returns>Модель</returns>
    public static T DeserializeData<T>(byte[] bytes)
    {
        var srt = Encoding.Default.GetString(bytes);
        var data = JsonConvert.DeserializeObject<T>(srt);
        return data;
    }
}