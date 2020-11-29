using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpMsg.Client
{
    public class Connection : INotifyPropertyChanged
    {
        private TcpClient _client = null;
        private Queue<byte[]> _newMessages = new Queue<byte[]>();

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

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

        public async Task StartListeningAsync()
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
                        //NumberOfMessages += "x";
                        //OnPropertyChanged("NumberOfMessages");

                        //using var ms = new MemoryStream(data);
                        //var bitmap = new BitmapImage();
                        //bitmap.BeginInit();
                        //bitmap.StreamSource = ms;
                        //bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        //bitmap.EndInit();
                        //var img = new Image();
                        //img.Source = bitmap;
                        //MessageGrid.Children.Add(img);
                        //image.Source = bitmap;
                    }
                }
                catch
                {
                    throw;
                }

                await Task.Delay(100);
            }
        }

        public async Task Send(byte[] data)
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

        public async Task SendDisconnectMessage()
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
