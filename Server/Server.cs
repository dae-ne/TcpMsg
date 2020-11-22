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
        //public static void Start()
        //{
        //    //var host = Dns.GetHostEntry("localhost");
        //    //var ipAddress = host.AddressList[0];
        //    var ipAddress = IPAddress.Parse("127.0.0.1");
        //    var localEndPoint = new IPEndPoint(ipAddress, 11000);

        //    //Console.WriteLine($"IP address: {localEndPoint.Address}");
        //    //Console.WriteLine($"Port: {localEndPoint.Port}");

        //    try
        //    {
        //        using var listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        //        listener.Bind(localEndPoint);
        //        listener.Listen(50);

        //        Console.WriteLine("Waiting for a connection...");

        //        for (; ; )
        //        {
        //            using var handler = listener.Accept();
        //            //Console.WriteLine(handler.RemoteEndPoint);
        //            var data = "";

        //            while (true)
        //            {
        //                var bytes = new byte[1024];
        //                var bytesRec = handler.Receive(bytes);
        //                data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

        //                if (data.IndexOf("<EOF>") > -1)
        //                {
        //                    break;
        //                }
        //            }

        //            Console.WriteLine($"Text received : {data}");
        //            var msg = Encoding.ASCII.GetBytes(data);
        //            //for (; ; )
        //            handler.Send(msg, SocketFlags.None);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.ToString());
        //    }
        //}

        //public static void Start()
        //{
        //    var connectionsManager = new ConnectionsManager();
        //    //var host = Dns.GetHostEntry("localhost");
        //    var ipAddress = IPAddress.Parse("127.0.0.1");
        //    var listener = new TcpListener(ipAddress, 11000);
        //    listener.Start(50);

        //    for (; ; )
        //    {
        //        try
        //        {
        //            var bytes = new byte[1024];
        //            var newClient = listener.AcceptTcpClient();
        //            connectionsManager.Register(new ClientSocket(connectionsManager, newClient));
        //            //using var stream = newClient.GetStream();
        //            //var length = stream.Read(bytes);
        //            //stream.Write(bytes, 0, length);
        //            //Console.WriteLine(Encoding.UTF8.GetString(bytes, 0, length));
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine(e.ToString());
        //        }
        //    }
        //}

        public void Main()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            var ipAddress = IPAddress.Parse("127.0.0.1");
            TcpListener listener = new TcpListener(ipAddress, 11000);
            try
            {
                listener.Start();
                //just fire and forget. We break from the "forgotten" async loops
                //in AcceptClientsAsync using a CancellationToken from `cts`
                AcceptClientsAsync(listener, cts.Token);
                //Thread.Sleep(60000); //block here to hold open the server
                Console.WriteLine("Press <enter> to quit");
                Console.ReadLine();
            }
            finally
            {
                cts.Cancel();
                listener.Stop();
            }
        }

        async Task AcceptClientsAsync(TcpListener listener, CancellationToken ct)
        {
            var clientCounter = 0;
            while (!ct.IsCancellationRequested)
            {
                TcpClient client = await listener.AcceptTcpClientAsync()
                                                    .ConfigureAwait(false);
                clientCounter++;
                //once again, just fire and forget, and use the CancellationToken
                //to signal to the "forgotten" async invocation.
                EchoAsync(client, clientCounter, ct);
            }

        }
        async Task EchoAsync(TcpClient client,
                             int clientIndex,
                             CancellationToken ct)
        {
            Console.WriteLine("New client ({0}) connected", clientIndex);
            using (client)
            {
                var buf = new byte[4096];
                var stream = client.GetStream();
                while (!ct.IsCancellationRequested)
                {
                    //under some circumstances, it's not possible to detect
                    //a client disconnecting if there's no data being sent
                    //so it's a good idea to give them a timeout to ensure that 
                    //we clean them up.
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
                    //now we know that the amountTask is complete so
                    //we can ask for its Result without blocking
                    var amountRead = amountReadTask.Result;
                    if (amountRead == 0) break; //end of stream.
                    await stream.WriteAsync(buf, 0, amountRead, ct)
                                .ConfigureAwait(false);
                }
            }
            Console.WriteLine("Client ({0}) disconnected", clientIndex);
        }
    }
}
