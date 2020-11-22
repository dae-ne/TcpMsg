using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ClientSocket : ICloneable
    {
        private readonly ConnectionsManager _connectionsManager;
        private readonly TcpClient _socket;

        public ClientSocket(ConnectionsManager connectionsManager, TcpClient socket)
        {
            _connectionsManager = connectionsManager;
            _socket = socket;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public async Task<byte[]> ListenAsync()
        {
            var bytes = new byte[1024];
            var length = 0;

            try
            {
                var stream = _socket.GetStream();
                length = await stream.ReadAsync(bytes)
                                         .ConfigureAwait(false);
                Console.WriteLine(Encoding.UTF8.GetString(bytes, 0, length));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                _connectionsManager.AddToUnregisterQueue(this);
            }

            return bytes.Where((_, i) => i < length).ToArray();
        }

        public async Task SendAsync(byte[] data, int length)
        {
            try
            {
                var stream = _socket.GetStream();
                await stream.WriteAsync(data, 0, length)
                            .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                _connectionsManager.AddToUnregisterQueue(this);
            }
        }
    }
}
