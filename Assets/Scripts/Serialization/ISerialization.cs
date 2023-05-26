using System.IO;

public interface ISerialization<T>
{
    string Format { get; }
    void Serialize(T data, Stream stream);
    T Deserialize(Stream stream);
}
