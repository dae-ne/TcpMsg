using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpMsg.Server
{
    class ClientSocket
    {
        private readonly ConnectionsManager _connectionsManager;
        private readonly TcpClient _socket;
        private const string CancelMsg = "<<disconnectme>>";

        private int CancelMsgSize => CancelMsg.Length;

        public ClientSocket(ConnectionsManager connectionsManager, TcpClient socket)
        {
            _connectionsManager = connectionsManager;
            _socket = socket;
            _connectionsManager.Register(this);
        }

        public async Task<byte[]> ListenAsync()
        {
            var bytes = new byte[1024];
            var length = 0;

            try
            {
                var stream = _socket.GetStream();
                length = await stream.ReadAsync(bytes);
                DisconnectAfterCancelMsg(bytes, length);
                var streamSize = BitConverter.ToInt32(bytes);

                if (length < 1 || streamSize < 1)
                {
                    return Array.Empty<byte>();
                }

                bytes = new byte[streamSize];
                length = await stream.ReadAsync(bytes);
                var responseData = Encoding.UTF8.GetString(bytes, 0, length);
                DisconnectAfterCancelMsg(bytes, length);

                if (streamSize != length)
                {
                    Console.WriteLine("There is a problem with a recived message");
                    return Array.Empty<byte>();
                }
            }
            catch
            {
                throw;
            }

            return bytes.Where((_, i) => i < length).ToArray();
        }

        public async Task SendAsync(byte[] data, int length)
        {
            try
            {
                var stream = _socket.GetStream();
                var streamSize = BitConverter.GetBytes(length);
                await stream.WriteAsync(streamSize);
                await stream.WriteAsync(data, 0, length);
            }
            catch
            {
                throw;
            }
        }

        public void CloseConnection()
        {
            _socket.GetStream().Close();
            _socket.Close();
            _connectionsManager.Unregister(this);
        }

        private void DisconnectAfterCancelMsg(byte[] data, int length)
        {
            if (length == CancelMsgSize)
            {
                var msg = Encoding.UTF8.GetString(data, 0, length);

                if (msg.Equals(CancelMsg))
                {
                    throw new Exception();
                }
            }
        }
    }
}
