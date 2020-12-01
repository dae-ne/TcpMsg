using System;
using System.Linq;
using System.Text;

namespace TcpMsg.Client.MsgEncoding
{
    class TextToObjectConverter : ToObjectConverter
    {
        public TextToObjectConverter()
        {
            _type = DataType.Text;
        }

        protected override Type ConvertThisType(byte[] data, out object dataToDisplay)
        {
            var newData = data.ToList();
            newData.RemoveRange(0, 4);
            dataToDisplay = Encoding.UTF8.GetString(newData.ToArray());
            return dataToDisplay.GetType();
        }
    }
}
