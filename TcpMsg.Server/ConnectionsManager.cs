using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TcpMsg.Server
{
    class ConnectionsManager
    {
        private readonly List<ClientSocket> _sockets = new List<ClientSocket>();

        public int NumberOfClients => _sockets.Count;

        public bool Contains(ClientSocket client) => _sockets.Contains(client);

        public void Register(ClientSocket socket) => _sockets.Add(socket);

        public bool Unregister(ClientSocket socket) => _sockets.Remove(socket);

        public async Task HandleConnectionAsync(ClientSocket socket)
        {
            if (_sockets.Contains(socket))
            {
                try
                {
                    var data = await socket.ListenAsync();

                    if (data.Length > 0)
                    {
                        await NotifyAllAsync(data, data.Length);
                    }
                }
                catch
                {
                    socket.CloseConnection();
                }
            }
        }

        public async Task NotifyAllAsync(byte[] data, int length)
        {
            foreach (var socket in _sockets.ToList())
            {
                await socket.SendAsync(data, length);
            }
        }
    }
}
