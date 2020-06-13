using Newtonsoft.Json;

namespace CC.DialogueSystem
{
    public class JSONDeserializer : IDeserializer
    {
        public T Deserialize<T>(string text) => JsonConvert.DeserializeObject<T>(text);
    }
}