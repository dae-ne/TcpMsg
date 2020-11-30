using System;
using System.Linq;
using TcpMsg.Client.Components;

namespace TcpMsg.Client.MsgEncoding
{
    class AudioToSendConverter : ToSendConverter
    {
        public AudioToSendConverter()
        {
            _type = DataType.Audio;
        }

        protected override byte[] ConvertThisType(object data)
        {
            var result = BitConverter.GetBytes((int)_type).ToList();
            result.AddRange(((Audio)data).Bytes);
            return result.ToArray();
        }
    }
}
