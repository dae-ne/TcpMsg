using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server
{
    class ClientSocket
    {
        private readonly ConnectionsManager _connectionsManager;
        private readonly TcpClient _socket;

        public ClientSocket(ConnectionsManager connectionsManager, TcpClient socket)
        {
            _connectionsManager = connectionsManager;
            _socket = socket;
            _connectionsManager.Register(this);
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
            }
            catch
            {
                throw;
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
            catch
            {
                throw;
            }
        }

        public void CloseConnection()
        {
            _socket.Close();
            _connectionsManager.Unregister(this);
        }
    }
}
