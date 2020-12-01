using Microsoft.Win32;
using System.IO;
using System.Threading.Tasks;

namespace TcpMsg.Client.FileIO
{
    class TextWriter : Writer
    {
        public TextWriter()
            : base(typeof(string), "txt file | *.txt")
        { }

        protected override async Task SaveData(SaveFileDialog saveFileDialog, object data)
        {
            if (saveFileDialog.ShowDialog() == true)
            {
                await File.WriteAllTextAsync(saveFileDialog.FileName, data as string);
            }
        }
    }
}
