using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TcpMsg.Client.Components
{
    class Connection
    {
        private TcpClient _client = null;
        private readonly Queue<byte[]> _newMessages = new Queue<byte[]>();

        public int NumberOfMessages => _newMessages.Count;

        public Connection() { }

        public Connection(string ip, int port)
        {
            try
            {
                Connect(ip, port);
            }
            catch
            {
                throw;
            }
        }

        public byte[] NextMessage()
        {
            try
            {
                return _newMessages.Dequeue();
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }

        public void Connect(string ip, int port)
        {
            try
            {
                _client = new TcpClient(ip, port);
            }
            catch
            {
                throw;
            }
        }

        public void Disconnect()
        {
            try
            {
                _client.GetStream().Close();
                _client.Close();
            }
            catch { }
        }

        public async Task StartListeningAsync(TextBlock messageCounter)
        {
            var stream = _client.GetStream();

            for (; ; )
            {
                try
                {
                    var data = new byte[1024];
                    var length = await stream.ReadAsync(data, 0, data.Length);

                    if (length > 0)
                    {
                        var streamSize = BitConverter.ToInt32(data);
                        data = new byte[streamSize];
                        length = await stream.ReadAsync(data, 0, data.Length);

                        if (streamSize != length)
                        {
                            throw new Exception("There is a problem with a recived message.");
                        }

                        _newMessages.Enqueue(data);
                        messageCounter.Text = NumberOfMessages.ToString();
                    }
                }
                catch
                {
                    throw;
                }

                await Task.Delay(100);
            }
        }

        public async Task SendAsync(byte[] data)
        {
            try
            {
                var stream = _client.GetStream();
                var streamSize = BitConverter.GetBytes(data.Length);
                await stream.WriteAsync(streamSize, 0, streamSize.Length);
                await stream.WriteAsync(data, 0, data.Length);
            }
            catch
            {
                throw;
            }
        }

        public async Task SendDisconnectMessageAsync()
        {
            try
            {
                var stream = _client.GetStream();
                var data = Encoding.UTF8.GetBytes("<<disconnectme>>");
                await stream.WriteAsync(data, 0, data.Length);
            }
            catch
            {
                throw;
            }
        }
    }
}
