using System;
using System.Linq;
using System.Text;

namespace TcpMsg.Client.MsgEncoding
{
    class TextToSendConverter : ToSendConverter
    {
        public TextToSendConverter()
        {
            _type = DataType.Text;
        }

        protected override byte[] ConvertThisType(object data)
        {
            var text = Encoding.UTF8.GetBytes(data as string).ToList();
            var dataTypeAsInt = BitConverter.GetBytes((int)_type);
            text.InsertRange(0, dataTypeAsInt);
            return text.ToArray();
        }
    }
}
