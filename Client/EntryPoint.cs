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
                stream.Write(data, 0, data.Length);
                Console.WriteLine($"sent: {message}");

                for (; ; )
                {
                    data = new byte[1024];
                    var bytes = stream.Read(data, 0, data.Length);

                    while (bytes == 0) { }

                    var responsedata = Encoding.UTF8.GetString(data, 0, bytes);
                    Console.WriteLine($"received: {responsedata}, {bytes}");
                    Task.Delay(3000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\npress enter to continue...");
            Console.Read();
        }
    }
}
