using System.Collections.Generic;

namespace Server
{
    class ConnectionsManager
    {
        private List<ClientSocket> _sockets = new List<ClientSocket>();

        private Queue<ClientSocket> _unregisterQueue = new Queue<ClientSocket>();

        public void Register(ClientSocket socket) => _sockets.Add(socket);

        public bool Unregister(ClientSocket socket) => _sockets.Remove(socket);

        public void AddToUnregisterQueue(ClientSocket clientSocket) => _unregisterQueue.Enqueue(clientSocket);

        public void UnregisterQueued()
        {
            while (_unregisterQueue.Count > 0)
            {
                var socket = _unregisterQueue.Dequeue();
                Unregister(socket);
            }
        }

        public void ListenToAll()
        {
            foreach (var socket in _sockets)
            {
                //_sockets.Listen
            }

            UnregisterQueued();
        }

        public void NotifyAll()
        {
            foreach (var socket in _sockets)
            {

            }

            UnregisterQueued();
        }
    }
}
