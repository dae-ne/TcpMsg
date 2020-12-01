using System;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace TcpMsg.Client.MsgEncoding
{
    class ImageToObjectConverter : ToObjectConverter
    {
        public ImageToObjectConverter()
        {
            _type = DataType.Image;
        }

        protected override Type ConvertThisType(byte[] data, out object dataToDisplay)
        {
            var newData = data.ToList();
            newData.RemoveRange(0, 4);
            using var ms = new MemoryStream(newData.ToArray());
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = ms;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            dataToDisplay = bitmap;
            return dataToDisplay.GetType();
        }
    }
}
