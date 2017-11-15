using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.RepresentationModel;

namespace HalfShot.MagnetHS.DataTransformer
{
    public class YAMLDataTransformer : IDataTransformer
    {
        public string ConvertToString(object obj)
        {
            var serializer = new SerializerBuilder().Build();
            return serializer.Serialize(obj);
        }

        public T FromStream<T>(TextReader stream)
        {
            var deserializer = new Deserializer();
            return deserializer.Deserialize<T>(stream);
        }

        public byte[] ToBytes(object obj)
        {
            return Encoding.UTF8.GetBytes(ConvertToString(obj));
        }

        public Stream ToStream(object obj)
        {
            var serializer = new SerializerBuilder().Build();
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            serializer.Serialize(writer, obj);
            writer.Flush();
            return memoryStream;
        }
    }
}
