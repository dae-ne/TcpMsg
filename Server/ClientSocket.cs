using System;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class ClientSocket
    {
        private readonly TcpClient _socket;

        public ClientSocket(TcpClient socket)
        {
            _socket = socket;
        }

        public void Listen()
        {
            try
            {
                var bytes = new byte[1024];
                using var stream = _socket.GetStream();
                var length = stream.Read(bytes);
                Console.WriteLine(Encoding.UTF8.GetString(bytes, 0, length));
            }
            catch
            {
                throw;
            }
        }

        public void Send(byte[] data, int length)
        {
            try
            {
                using var stream = _socket.GetStream();
                stream.Write(data, 0, length);
            }
            catch
            {
                throw;
            }
        }
    }
}
