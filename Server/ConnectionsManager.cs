using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server
{
    class ConnectionsManager
    {
        private readonly List<ClientSocket> _sockets = new List<ClientSocket>();

        public bool Contains(ClientSocket client) => _sockets.Contains(client);

        public void Register(ClientSocket socket) => _sockets.Add(socket);

        public bool Unregister(ClientSocket socket) => _sockets.Remove(socket);

        public async Task NotifyAll(byte[] data, int length)
        {
            foreach (var socket in _sockets)
            {
                await socket.SendAsync(data, length);
            }
        }
    }
}
