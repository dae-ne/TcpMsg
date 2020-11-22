using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class EntryPoint
    {
        static void Main()
        {
            StartClient();
        }

        public static void StartClient()
        {
            try
            {
                var message = "my message";
                using var client = new TcpClient("127.0.0.1", 11000);
                using var stream = client.GetStream();
                var data = Encoding.UTF8.GetBytes(message);
                //stream.Write(data, 0, data.Length);
                //Console.WriteLine($"sent: {message}");
                //data = new byte[1024];
                //var bytes = stream.read(data, 0, data.length);
                //var responsedata = encoding.ascii.getstring(data, 0, bytes);
                //console.writeline($"received: {responsedata}");

                for (; ; )
                {
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine($"sent: {message}");
                    data = new byte[1024];
                    var bytes = stream.Read(data, 0, data.Length);

                    while (bytes == 0) { }

                    var responsedata = Encoding.UTF8.GetString(data, 0, bytes);
                    Console.WriteLine($"received: {responsedata}, {bytes}");

                    Task.Delay(3000);
                }

                // close everything.
                //stream.close();
                //client.close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("argumentnullexception: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("socketexception: {0}", e);
            }

            Console.WriteLine("\npress enter to continue...");
            Console.Read();
        }

        //public static void StartClient()
        //{
        //    byte[] bytes = new byte[1024];

        //    try
        //    {
        //        // Connect to a Remote server  
        //        // Get Host IP Address that is used to establish a connection  
        //        // In this case, we get one IP address of localhost that is IP : 127.0.0.1  
        //        // If a host has multiple addresses, you will get a list of addresses  
        //        IPHostEntry host = Dns.GetHostEntry("localhost");
        //        //IPAddress ipAddress = host.AddressList[0];
        //        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        //        IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

        //        // Create a TCP/IP  socket.    
        //        Socket sender = new Socket(ipAddress.AddressFamily,
        //            SocketType.Stream, ProtocolType.Tcp);

        //        // Connect the socket to the remote endpoint. Catch any errors.    
        //        try
        //        {
        //            // Connect to Remote EndPoint  
        //            sender.Connect(remoteEP);

        //            Console.WriteLine("Socket connected to {0}",
        //                sender.RemoteEndPoint.ToString());

        //            // Encode the data string into a byte array.    
        //            byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");

        //            // Send the data through the socket.    
        //            int bytesSent = sender.Send(msg);

        //            for (; ; )
        //            {
        //                // Receive the response from the remote device.    
        //                int bytesRec = sender.Receive(bytes);
        //                Console.WriteLine("Echoed test = {0}",
        //                    Encoding.ASCII.GetString(bytes, 0, bytesRec));
        //            }

        //            // Release the socket.    
        //            sender.Shutdown(SocketShutdown.Both);
        //            sender.Close();

        //        }
        //        catch (ArgumentNullException ane)
        //        {
        //            Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
        //        }
        //        catch (SocketException se)
        //        {
        //            Console.WriteLine("SocketException : {0}", se.ToString());
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine("Unexpected exception : {0}", e.ToString());
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.ToString());
        //    }
        //}
    }
}
