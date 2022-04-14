namespace SocketShared.Config;

/// <summary>
/// Ini конфиг
/// </summary>
public class IniConfig
{
    /// <summary>
    /// IP адрес
    /// </summary>
    public string Ip { get; private set; }
    
    /// <summary>
    /// Порт
    /// </summary>
    public int Port { get; private set; }
    
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="path">Путь к ini конфигу</param>
    public IniConfig(string path)
    {
        ParseIni(path);
    }

    private void ParseIni(string path)
    {
        var lines = File
            .ReadAllLines(path)
            .Select(l => l.Split("="))
            .ToDictionary(key => key[0], val => val[1]);

        if (!lines.TryGetValue("IP", out var ip) || string.IsNullOrEmpty(ip))
            throw new Exception($"Пустой IP в конфиге");

        if (!lines.TryGetValue("PORT", out var port) || string.IsNullOrEmpty(port))
            throw new Exception("Пустой PORT в конфиге");

        if(!int.TryParse(port, out var portValue))
            throw new Exception("PORT должен быть числом");
        
        this.Ip = ip;
        this.Port = portValue;
    }
}