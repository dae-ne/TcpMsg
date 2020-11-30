using System;
using System.Linq;
using TcpMsg.Client.Components;

namespace TcpMsg.Client.MsgEncoding
{
    class AudioToDisplayConverter : ToDisplayConverter
    {
        public AudioToDisplayConverter()
        {
            _type = DataType.Audio;
        }

        protected override Type ConvertThisType(byte[] data, out object dataToDisplay)
        {
            var newData = data.ToList();
            newData.RemoveRange(0, 4);
            dataToDisplay = new Audio(newData.ToArray());
            return dataToDisplay.GetType();
        }
    }
}
