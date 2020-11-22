using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        private ConnectionsManager _connections = new ConnectionsManager();

        public async Task Run(string ip, int port)
        {
            var cts = new CancellationTokenSource();
            var ipAddress = IPAddress.Parse(ip);
            TcpListener listener = new TcpListener(ipAddress, port);

            try
            {
                listener.Start();
                _ = AcceptClientsAsync(listener, cts.Token);
                _ = StartListeningAsync(cts.Token);
                Console.WriteLine("Server started");
                Console.WriteLine("Waiting for connections . . .");
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
                var newClientObject = new ClientSocket(_connections, client);
                _connections.Register(newClientObject);
                Console.WriteLine($"New client ({clientCounter++}) connected");
                //clientCounter++;
                //_ = EchoAsync(client, clientCounter, ct);
            }
        }

        private async Task StartListeningAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await _connections.ListenToAllAsync()
                                  .ConfigureAwait(false);
            }
        }

        private async Task EchoAsync(TcpClient client,
                             int clientIndex,
                             CancellationToken ct)
        {
            Console.WriteLine($"New client ({clientIndex}) connected");

            using (client)
            {
                var buf = new byte[4096];
                var stream = client.GetStream();

                while (!ct.IsCancellationRequested)
                {
                    var timeoutTask = Task.Delay(TimeSpan.FromSeconds(15));
                    var amountReadTask = stream.ReadAsync(buf, 0, buf.Length, ct);
                    var completedTask = await Task.WhenAny(timeoutTask, amountReadTask)
                                                  .ConfigureAwait(false);

                    if (completedTask == timeoutTask)
                    {
                        var msg = Encoding.ASCII.GetBytes("Client timed out");
                        await stream.WriteAsync(msg, 0, msg.Length);
                        break;
                    }

                    var amountRead = amountReadTask.Result;
                    if (amountRead == 0) break;
                    await stream.WriteAsync(buf, 0, amountRead, ct)
                                .ConfigureAwait(false);
                }
            }

            Console.WriteLine($"Client ({clientIndex}) disconnected");
        }
    }
}
