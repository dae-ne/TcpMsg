using System;
using System.Linq;
using System.Text;

namespace TcpMsg.Client.MsgEncoding
{
    class TextToDisplayConverter : ToDisplayConverter
    {
        public TextToDisplayConverter()
        {
            _type = DataType.Text;
        }

        protected override Type ConvertThisType(byte[] data, out object dataToDisplay)
        {
            var newData = data.ToList();
            newData.RemoveAt(0);
            dataToDisplay = Encoding.UTF8.GetString(data);
            return dataToDisplay.GetType();
        }
    }
}
