using Newtonsoft.Json;

public class JSONDeserializer : IDeserializer
{
    public T Deserialize<T>(string text)
    {
        return JsonConvert.DeserializeObject<T>(text);
    }
}