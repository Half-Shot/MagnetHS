using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using MsgPack;
using MsgPack.Serialization;

namespace HalfShot.MagnetHS.DataTransformer
{
    public class MsgPackDataTransformer : IDataTransformer
    {
        public byte[] ToBytes(object obj)
        {
            var serial = GetSerializer(obj.GetType());
            return serial.PackSingleObject(obj);
        }

        public string ConvertToString(object obj)
        {
            //Msgpack is a byteformat, so lets just base64 it.
            return Convert.ToBase64String(ToBytes(obj));
        }

        public Stream ToStream(object obj)
        {
            var serial = GetSerializer(obj.GetType());
            var stream = new MemoryStream();
            serial.PackTo(Packer.Create(stream, false), obj);
            return stream;
        }

        public T FromString<T>(string data)
        {
            byte[] byteData = Convert.FromBase64String(data);
            return FromBytes<T>(byteData);
        }

        public T FromBytes<T>(byte[] data)
        {
            return (T)GetSerializer(typeof(T)).UnpackSingleObject(data);
        }

        public T FromStream<T>(TextReader stream)
        {
            return FromString<T>(stream.ReadToEnd());
        }

        private static MessagePackSerializer GetSerializer(Type t)
        {
            return SerializationContext.Default.GetSerializer(t);
        }
    }
}