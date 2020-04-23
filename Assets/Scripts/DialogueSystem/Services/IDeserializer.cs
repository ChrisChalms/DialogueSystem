public interface IDeserializer
{
    T Deserialize<T>(string text);
}
