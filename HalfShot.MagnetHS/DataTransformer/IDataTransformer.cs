using System;
using System.Threading.Tasks;
using System.IO;
namespace HalfShot.MagnetHS.DataTransformer
{
    public interface IDataTransformer
    {
        byte[] ToBytes(object obj);
        string ConvertToString(object obj);
        Stream ToStream(object obj);
        T FromString<T>(string data);
        T FromStream<T>(TextReader stream);
    }
}
