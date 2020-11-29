using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace TcpMsg.Client.MsgEncoding
{
    abstract class ToSendConverter
    {
        //protected Type _type;
        protected DataType _type;
        protected ToSendConverter _nextConverter = null;

        protected static readonly Dictionary<Type, int> _numbersForTypes
            = new Dictionary<Type, int>
            {
                { typeof(string), (int)DataType.Text },
                { typeof(BitmapImage), (int)DataType.Image },
                { typeof(int), (int)DataType.Audio }
            };

        public void SetNextConverter(ToSendConverter nextConverter)
        {
            _nextConverter = nextConverter;
        }

        public byte[] Convert(object data)
        {
            if (_numbersForTypes[data.GetType()] == (int)_type)
            {
                return ConvertThisType(data);
            }
            else if (_nextConverter != null)
            {
                return _nextConverter.Convert(data);
            }

            throw new Exception("Wrong data type");
        }

        abstract protected byte[] ConvertThisType(object data);
    }
}
