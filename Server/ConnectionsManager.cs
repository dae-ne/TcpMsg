using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    class ConnectionsManager
    {
        private readonly List<ClientSocket> _sockets = new List<ClientSocket>();
        private readonly Queue<ClientSocket> _unregisterQueue = new Queue<ClientSocket>();

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

        public async Task ListenToAllAsync()
        {
            if (_sockets.Count < 1)
            {
                await Task.Delay(200);
            }
            else
            {
                if (_sockets.Count > 1)
                {
                    Console.WriteLine("asdf");
                }

                foreach (var socket in _sockets)
                {
                    var bytes = await socket.ListenAsync();

                    if (bytes.Length > 0)
                    {
                        await NotifyAll(bytes, bytes.Length);
                    }
                }

                UnregisterQueued();
            }
        }

        public async Task NotifyAll(byte[] data, int length)
        {
            foreach (var socket in _sockets)
            {
                await socket.SendAsync(data, length);
            }
        }
    }
}
