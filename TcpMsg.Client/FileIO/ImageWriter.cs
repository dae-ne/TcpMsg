using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TcpMsg.Client.MsgEncoding;

namespace TcpMsg.Client.FileIO
{
    class ImageWriter : Writer
    {
        public ImageWriter()
            : base(typeof(BitmapImage), "Bitmap Image | *.bmp")
        { }

        protected override async Task SaveData(SaveFileDialog saveFileDialog, object data)
        {
            if (saveFileDialog.ShowDialog() == true)
            {
                var converter = new AudioToBytesConverter();
                var bytes = converter.Convert(data as BitmapImage);
                bytes = bytes.Where((_, i) => i > 3).ToArray();
                await File.WriteAllBytesAsync(saveFileDialog.FileName, bytes);
            }
        }
    }
}
