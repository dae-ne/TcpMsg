using System.IO;
using System.Threading.Tasks;

namespace TcpMsg.Client.Audio
{
    public class Audio
    {
        public byte[] Bytes { get; private set; }

        public Audio() { }

        public Audio(byte[] data)
        {
            CreateFromBytes(data);
        }

        public void CreateFromBytes(byte[] data)
        {
            Bytes = data;
        }

        public async Task LoadFromFileAsync(string uri)
        {
            try
            {
                Bytes = await File.ReadAllBytesAsync(uri);
            }
            catch
            {
                throw;
            }
        }

        public async Task SaveFileAsync(string uri)
        {
            try
            {
                await File.WriteAllBytesAsync(uri, Bytes);
            }
            catch
            {
                throw;
            }
        }

        public MemoryStream GetMemoryStream()
        {
            return new MemoryStream(Bytes, true);
        }
    }
}
