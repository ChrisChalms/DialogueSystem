using Newtonsoft.Json;

namespace CC.DialogueSystem
{
    public class JSONDeserializer : IDeserializer
    {
        public T Deserialize<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }
    }
}