using System;
using System.Linq;
using TcpMsg.Client.Media;

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
            result.AddRange(((Media.Audio)data).Bytes);
            return result.ToArray();
        }
    }
}
