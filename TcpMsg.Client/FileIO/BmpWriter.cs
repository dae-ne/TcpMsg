using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TcpMsg.Client.MsgEncoding;

namespace TcpMsg.Client.FileIO
{
    class BmpWriter : Writer
    {
        public BmpWriter()
            : base(typeof(BitmapImage), "Bitmap Image | *.bmp")
        { }

        protected override async Task SaveData(SaveFileDialog saveFileDialog, object data)
        {
            if (saveFileDialog.ShowDialog() == true)
            {
                var converter = new ImageToBytesConverter();
                var bytes = converter.Convert(data);
                bytes = bytes.Where((_, i) => i > 3).ToArray();
                await File.WriteAllBytesAsync(saveFileDialog.FileName, bytes);
            }
        }
    }
}
