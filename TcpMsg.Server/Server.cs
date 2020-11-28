using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TcpMsg.Server
{
    class Server
    {
        private readonly ConnectionsManager _connectionsManager = new ConnectionsManager();

        public void Run(string ip, int port)
        {
            var cts = new CancellationTokenSource();
            var ipAddress = IPAddress.Parse(ip);
            var listener = new TcpListener(ipAddress, port);

            try
            {
                listener.Start();
                Console.WriteLine("Server started");
                Console.WriteLine("Waiting for connections . . .");
                _ = AcceptClientsAsync(listener, cts.Token);
                Console.WriteLine("Press <enter> to quit\n");
                Console.ReadLine();
            }
            finally
            {
                cts.Cancel();
                listener.Stop();
            }
        }

        private async Task AcceptClientsAsync(TcpListener listener, CancellationToken ct)
        {
            var clientCounter = 0u;

            while (!ct.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync()
                                           .ConfigureAwait(false);
                clientCounter++;
                _ = HandleConnection(client, ct, clientCounter);
            }
        }

        private async Task HandleConnection(TcpClient client, CancellationToken ct, uint clientIndex)
        {
            Console.WriteLine($"New client (id: {clientIndex}) connected");
            var newClientObject = new ClientSocket(_connectionsManager, client);

            while (_connectionsManager.Contains(newClientObject)
                && !ct.IsCancellationRequested)
            {
                await _connectionsManager.HandleConnectionAsync(newClientObject);
                await Task.Delay(100);
            }

            Console.WriteLine($"Client (id: {clientIndex}) disconnected");
        }
    }
}
