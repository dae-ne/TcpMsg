using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using TcpMsg.Client.MsgEncoding;

namespace TcpMsg.Client
{
    public partial class MainWindow : Window
    {
        //private const string DisconnectMessage = "<<disconnectme>>";
        //private TcpClient _client = null;
        private Connection _connection;
        private ToSendConverter _toSendConverter;
        private ToDisplayConverter _toDisplayConverter;
        private bool _isSending = false;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = _connection;
            SetEncodingChains();

            try
            {
                //_client = new TcpClient("127.0.0.1", 11000);
                _connection = new Connection("127.0.0.1", 11000);
                Closed += new EventHandler(MainWindow_Closed);
                _ = _connection.StartListeningAsync();
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\n\n" +
                    $"Check if the server is running.\n" +
                    $"Application will be closed.", e.GetType().Name);
                Application.Current.Shutdown();
            }
        }

        //private async Task StartListeningAsync()
        //{
        //    var stream = _client.GetStream();

        //    for (; ; )
        //    {
        //        try
        //        {
        //            var data = new byte[1024];
        //            var length = await stream.ReadAsync(data, 0, data.Length);

        //            if (length > 0)
        //            {
        //                var streamSize = BitConverter.ToInt32(data);
        //                data = new byte[streamSize];
        //                length = await stream.ReadAsync(data, 0, data.Length);

        //                if (streamSize != length)
        //                {
        //                    MessageBox.Show("There is a problem with a recived message.");
        //                }

        //                using var ms = new MemoryStream(data);
        //                var bitmap = new BitmapImage();
        //                bitmap.BeginInit();
        //                bitmap.StreamSource = ms;
        //                bitmap.CacheOption = BitmapCacheOption.OnLoad;
        //                bitmap.EndInit();
        //                var img = new Image();
        //                img.Source = bitmap;
        //                MessageGrid.Children.Add(img);
        //                //image.Source = bitmap;
        //            }
        //        }
        //        catch
        //        {
        //            Application.Current.Shutdown();
        //        }
                
        //        await Task.Delay(300);
        //    }
        //}

        private async void MainWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                await _connection.SendDisconnectMessage();
                //_client.GetStream().Close();
                //_client.Close();
                _connection.Disconnect();
            }
            catch { }
        }

        private void SelectPictureBt_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";

            if (openFileDialog.ShowDialog() == true)
            {
                ImgUriTextBlock.Text = openFileDialog.FileName;
            }

            AudioUriTextBlock.Text = "";
            MsgTextBox.Text = "";
        }

        private void SelectAudioFileBt_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Audio Files(*.mp3, *.wav, *.acc, *wma) | *.mp3; *.wav; *.acc; *.wma";

            if (openFileDialog.ShowDialog() == true)
            {
                AudioUriTextBlock.Text = openFileDialog.FileName;
            }

            ImgUriTextBlock.Text = "";
            MsgTextBox.Text = "";
        }

        private void MsgTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (MsgTextBox.Text != "")
            {
                ImgUriTextBlock.Text = "";
                AudioUriTextBlock.Text = "";
            }
        }

        private async void SendBt_Click(object sender, RoutedEventArgs e)
        {
            //MsgTextBox.Text = "";
            object message = null;

            //if (!_isSending)
            //{
                //_isSending = true;

            if (MsgTextBox.Text != "")
            {
                //var message =_toSendConverter();
                //await SendMessage(MsgTextBox.Text);
                message = MsgTextBox.Text;
                MsgTextBox.Text = "";
            }
            else if (ImgUriTextBlock.Text != "")
            {
                //var bitmapImage = new BitmapImage(new Uri(ImgUriTextBlock.Text));
                //var encoder = new JpegBitmapEncoder();
                //encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                //using MemoryStream ms = new MemoryStream();
                //encoder.Save(ms);
                //var data = ms.ToArray();
                ////await SendBytes(data);
                //await _connection.Send(data);
                message = new BitmapImage(new Uri(ImgUriTextBlock.Text));
                ImgUriTextBlock.Text = "";
            }

            if (message != null)
            {
                try
                {
                    var convertedMessage = _toSendConverter.Convert(message);
                    await _connection.Send(convertedMessage);
                }
                catch { }
            }

                //await Task.Delay(2000);
                //_isSending = false;
            //}

            //await SendMessage(MsgTextBox.Text);
        }

        private void NextMsgBt_Click(object sender, RoutedEventArgs e)
        {
            var message = _connection.NextMessage();

            if (message.Length > 0)
            {
                var messageType = _toDisplayConverter.Convert(message, out object receivedData);

                if (messageType == typeof(string))
                {
                    textBlock.Text = receivedData as string;
                }
                else if (messageType == typeof(BitmapImage))
                {
                    image.Source = receivedData as BitmapImage;
                }
                else if (messageType == typeof(int))
                {

                }
            }
        }

        private void SetEncodingChains()
        {
            _toSendConverter = new TextToSendConverter();
            var imgToSendConverter = new ImageToSendConverter();
            _toSendConverter.SetNextConverter(imgToSendConverter);

            _toDisplayConverter = new TextToDisplayConverter();
            var imgToDisplayConverter = new ImageToDisplayConverter();
            _toDisplayConverter.SetNextConverter(imgToDisplayConverter);
        }

        //private async Task SendMessage(string message, bool withSize = true)
        //{
        //    var data = Encoding.UTF8.GetBytes(message);
        //    await SendBytes(data, withSize);
        //}

        //private async Task SendBytes(byte[] data, bool withSize = true)
        //{
        //    try
        //    {
        //        var stream = _client.GetStream();

        //        if (withSize)
        //        {
        //            var streamSize = BitConverter.GetBytes(data.Length);
        //            await stream.WriteAsync(streamSize, 0, streamSize.Length);
        //        }

        //        await stream.WriteAsync(data, 0, data.Length);
        //    }
        //    catch
        //    {
        //        Application.Current.Shutdown();
        //    }
        //}
    }
}
