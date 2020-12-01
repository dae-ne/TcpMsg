using Microsoft.Win32;
using System.IO;
using System.Threading.Tasks;
using TcpMsg.Client.Media;

namespace TcpMsg.Client.FileIO
{
    class WavWriter : Writer
    {
        public WavWriter()
            : base(typeof(Audio), "Waveform | *.wav")
        { }

        protected override async Task SaveData(SaveFileDialog saveFileDialog, object data)
        {
            if (saveFileDialog.ShowDialog() == true)
            {
                var bytes = ((Audio)data).Bytes;
                await File.WriteAllBytesAsync(saveFileDialog.FileName, bytes);
            }
        }
    }
}
