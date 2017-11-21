using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace HalfShot.MagnetHS.DataTransformer
{
    public class JSONDataTransformer : IDataTransformer
    {
        public T FromStream<T>(TextReader stream)
        {
            return FromString<T>(stream.ReadToEnd());
        }

        public T FromString<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public string ConvertToString(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None);
        }

        public byte[] ToBytes(object obj)
        {
            return Encoding.UTF8.GetBytes(ConvertToString(obj));
        }

        public Stream ToStream(object obj)
        {
            return new MemoryStream(ToBytes(obj));
        }

    }
}
