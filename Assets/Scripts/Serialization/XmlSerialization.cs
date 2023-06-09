//USING_ZENJECT
using System.IO;
using System.Xml.Serialization;
#if !USING_ZENJECT
using UnityEngine;
#endif

public class XmlSerialization<T> :
#if !USING_ZENJECT
    MonoBehaviour,
#endif
    ISerialization<T>
{
    public string Format => "xml";

    public T Deserialize(Stream stream)
    {
        return (T)new XmlSerializer(typeof(T)).Deserialize(stream);
    }

    public void Serialize(T data, Stream stream)
    {
        var serializer = new XmlSerializer(typeof(T));
        serializer.Serialize(stream, data);
    }
}
