using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace HalfShot.MagnetHS.MessageQueue
{
    [Serializable]
    public abstract class MQItem
    {
        private byte[] SIGNATURE = new byte[4] { 0x86, 0x36, 0xcc, 0xfa } ;
        protected byte requestType = 0x00;

        public byte RequestType { get { return requestType; } }

        public MQItem()
        {

        }

        public MQItem(byte[] data)
        {

        }

        public byte[] GetHeader()
        {
            List<byte> bytes = new List<byte>(SIGNATURE);
            bytes.Add(requestType);
            return bytes.ToArray();
        }

        public static T FromBytes<T>(byte[] data) where T : MQItem
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binForm = new BinaryFormatter();
                memStream.Write(data, 0, data.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                return (T)binForm.Deserialize(memStream);
            }
        }

        public byte[] GetBytes()
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, this);
                return ms.ToArray();
            }
        }
    }
}
