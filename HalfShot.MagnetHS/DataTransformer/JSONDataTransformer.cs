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
            return JsonConvert.DeserializeObject<T>(stream.ReadToEnd());
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
