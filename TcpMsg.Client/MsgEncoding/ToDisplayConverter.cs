using System;
using System.Linq;

namespace TcpMsg.Client.MsgEncoding
{
    abstract class ToDisplayConverter
    {
        protected DataType _type;
        protected ToDisplayConverter _nextConverter = null;

        public void SetNextConverter(ToDisplayConverter nextConverter)
        {
            _nextConverter = nextConverter;
        }

        public Type Convert(byte[] data, out object dataToDisplay)
        {
            if (GetDataType(data) == (int)_type)
            {
                return ConvertThisType(data, out dataToDisplay);
            }
            else if (_nextConverter != null)
            {
                return _nextConverter.Convert(data, out dataToDisplay);
            }

            throw new Exception("Wrong data type");
        }

        private int GetDataType(byte[] data)
        {
            var arrayData = data.Where((_, i) => i < 4).ToArray();
            return BitConverter.ToInt32(arrayData);
        }

        abstract protected Type ConvertThisType(byte[] data, out object dataToDisplay);
    }
}
