using System;
using System.Linq;
using TcpMsg.Client.Audio;

namespace TcpMsg.Client.MsgEncoding
{
    class AudioToBytesConverter : ToBytesConverter
    {
        public AudioToBytesConverter()
        {
            _type = DataType.Audio;
        }

        protected override byte[] ConvertThisType(object data)
        {
            var result = BitConverter.GetBytes((int)_type).ToList();
            result.AddRange(((Audio.Audio)data).Bytes);
            return result.ToArray();
        }
    }
}
