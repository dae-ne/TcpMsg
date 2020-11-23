using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        private ConnectionsManager _connections = new ConnectionsManager();

        public void Run(string ip, int port)
        {
            var cts = new CancellationTokenSource();
            var ipAddress = IPAddress.Parse(ip);
            TcpListener listener = new TcpListener(ipAddress, port);

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
            var clientCounter = 0;

            while (!ct.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync()
                                                 .ConfigureAwait(false);
                clientCounter++;
                _ = ManageConnection(client, ct, clientCounter);
            }
        }

        private async Task ManageConnection(TcpClient client, CancellationToken ct, int clientIndex)
        {
            Console.WriteLine($"New client (id: {clientIndex}) connected");
            var newClientObject = new ClientSocket(_connections, client);
            _connections.Register(newClientObject);
            var stream = client.GetStream();

            while (_connections.Contains(newClientObject)
                && !ct.IsCancellationRequested)
            {
                try
                {
                    var buf = new byte[4096];
                    var length = await stream.ReadAsync(buf, 0, buf.Length, ct);

                    if (length > 0)
                    {
                        await stream.WriteAsync(buf, 0, length, ct)
                                    .ConfigureAwait(false);
                    }
                }
                catch
                {
                    _connections.Unregister(newClientObject);
                }

                await Task.Delay(100);
            }

            Console.WriteLine($"Client (id: {clientIndex}) disconnected");
        }
    }
}
