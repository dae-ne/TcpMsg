using System;
using System.Linq;
using TcpMsg.Client.Media;

namespace TcpMsg.Client.MsgEncoding
{
    class AudioToObjectConverter : ToObjectConverter
    {
        public AudioToObjectConverter()
        {
            _type = DataType.Audio;
        }

        protected override Type ConvertThisType(byte[] data, out object dataToDisplay)
        {
            var newData = data.ToList();
            newData.RemoveRange(0, 4);
            dataToDisplay = new Media.Audio(newData.ToArray());
            return dataToDisplay.GetType();
        }
    }
}
