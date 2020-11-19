using System.Collections.Generic;

namespace Server
{
    class ConnectionsManager
    {
        private List<ClientSocket> _sockets = new List<ClientSocket>();

        public void Register(ClientSocket socket) => _sockets.Add(socket);

        public void Unregister(ClientSocket socket) => _sockets.Remove(socket);

        public void ListenToAll()
        {
            foreach (var socket in _sockets)
            {
                //_sockets.Listen
            }
        }

        public void NotifyAll()
        {
            foreach (var socket in _sockets)
            {

            }
        }
    }
}
