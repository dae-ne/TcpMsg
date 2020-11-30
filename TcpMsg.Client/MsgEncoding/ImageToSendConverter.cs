using System.IO;
using System.Windows.Media.Imaging;
using System.Linq;
using System;

namespace TcpMsg.Client.MsgEncoding
{
    class ImageToSendConverter : ToSendConverter
    {
        public ImageToSendConverter()
        {
            _type = DataType.Image;
        }

        protected override byte[] ConvertThisType(object data)
        {
            var encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(data as BitmapImage));
            using MemoryStream ms = new MemoryStream();
            encoder.Save(ms);
            var img = ms.ToArray();
            var result = BitConverter.GetBytes((int)_type).ToList();
            result.AddRange(img);
            return result.ToArray();
        }
    }
}
