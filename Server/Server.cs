using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

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

        public static void Start()
        {
            var connectionsManager = new ConnectionsManager();
            //var host = Dns.GetHostEntry("localhost");
            var ipAddress = IPAddress.Parse("127.0.0.1");
            var listener = new TcpListener(ipAddress, 11000);
            listener.Start(50);

            for (; ; )
            {
                try
                {
                    //var bytes = new byte[1024];
                    var newClient = listener.AcceptTcpClient();
                    connectionsManager.Register(new ClientSocket(newClient));
                    //using var stream = newClient.GetStream();
                    //var length = stream.Read(bytes);
                    //stream.Write(bytes, 0, length);
                    //Console.WriteLine(Encoding.UTF8.GetString(bytes, 0, length));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
