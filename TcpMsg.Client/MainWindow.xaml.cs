using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TcpMsg.Client.FileIO;
using TcpMsg.Client.Media;
using TcpMsg.Client.MsgEncoding;
using TcpMsg.Client.Pages;

namespace TcpMsg.Client
{
    public partial class MainWindow : Window
    {
        private const string IpAddress = "127.0.0.1";
        private const int Port = 11000;
        private readonly Connection _connection;
        private ToBytesConverter _toBytesConverter;
        private ToObjectConverter _toObjectConverter;
        private bool _isSending = false;
        private object _currentMessage = null;

        public MainWindow()
        {
            InitializeComponent();
            SetEncodingChains();
            messageCounter.Text = "0";
            
            try
            {
                _connection = new Connection(IpAddress, Port);
                Closed += new EventHandler(MainWindow_Closed);
                _ = _connection.StartListeningAsync(messageCounter);
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\n\n" +
                    $"Check if the server is running.\n" +
                    $"Application will be closed.", e.GetType().Name);
                Application.Current.Shutdown();
            }
        }

        private async void MainWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                await _connection.SendDisconnectMessageAsync();
                _connection.Disconnect();
            }
            catch { }
        }

        private void SelectPictureBt_Click(object sender, RoutedEventArgs e)
        {
            var uriReader = new Reader(DataType.Image);
            ImgUriTextBlock.Text = uriReader.GetUriDialog();
            AudioUriTextBlock.Text = "";
            MsgTextBox.Text = "";
        }

        private void SelectAudioFileBt_Click(object sender, RoutedEventArgs e)
        {
            var uriReader = new Reader(DataType.Audio);
            AudioUriTextBlock.Text = uriReader.GetUriDialog();
            ImgUriTextBlock.Text = "";
            MsgTextBox.Text = "";
        }

        private void MsgTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MsgTextBox.Text != "")
            {
                ImgUriTextBlock.Text = "";
                AudioUriTextBlock.Text = "";
            }
        }

        private async void SendBt_Click(object sender, RoutedEventArgs e)
        {
            object message = null;

            if (!_isSending)
            {
                _isSending = true;

                if (MsgTextBox.Text != "")
                {
                    message = MsgTextBox.Text;
                    MsgTextBox.Text = "";
                }
                else if (ImgUriTextBlock.Text != "")
                {
                    message = new BitmapImage(new Uri(ImgUriTextBlock.Text));
                    ImgUriTextBlock.Text = "";
                }
                else if (AudioUriTextBlock.Text != "")
                {
                    var audio = new Audio();

                    try
                    {
                        await audio.LoadFromFileAsync(AudioUriTextBlock.Text);
                    }
                    catch
                    {
                        throw;
                    }

                    message = audio;
                    AudioUriTextBlock.Text = "";
                }

                if (message != null)
                {
                    try
                    {
                        var convertedMessage = _toBytesConverter.Convert(message);
                        await _connection.SendAsync(convertedMessage);
                    }
                    catch { }
                }

                await Task.Delay(2000);
                _isSending = false;
            }
        }

        private void NextMsgBt_Click(object sender, RoutedEventArgs e)
        {
            var message = _connection.NextMessage();
            messageCounter.Text = _connection.NumberOfMessages.ToString();

            if (message.Length > 0)
            {
                var messageType = _toObjectConverter.Convert(message, out object receivedData);
                _currentMessage = receivedData;

                if (messageType == typeof(string))
                {
                    Main.Content = new TextPage(receivedData as string);
                }
                else if (messageType == typeof(BitmapImage))
                {
                    Main.Content = new ImagePage(receivedData as BitmapImage);
                }
                else if (messageType == typeof(Audio))
                {
                    Main.Content = new AudioPage(receivedData as Media.Audio);
                }
            }
        }

        private async void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            if (_currentMessage.GetType() == typeof(string))
            {
                var writer = new TxtWriter();
                await writer.SaveToFile(_currentMessage);
            }
            else if (_currentMessage.GetType() == typeof(BitmapImage))
            {
                var writer = new BmpWriter();
                await writer.SaveToFile(_currentMessage);
            }
            else if (_currentMessage.GetType() == typeof(Audio))
            {
                var writer = new WavWriter();
                await writer.SaveToFile(_currentMessage);
            }
        }

        private void SetEncodingChains()
        {
            _toBytesConverter = new TextToBytesConverter();
            var imgToSendConverter = new ImageToBytesConverter();
            var audioToSendConverter = new AudioToBytesConverter();
            _toBytesConverter.SetNextConverter(imgToSendConverter);
            imgToSendConverter.SetNextConverter(audioToSendConverter);

            _toObjectConverter = new TextToObjectConverter();
            var imgToDisplayConverter = new ImageToObjectConverter();
            var audioToDisplayConverter = new AudioToObjectConverter();
            _toObjectConverter.SetNextConverter(imgToDisplayConverter);
            imgToDisplayConverter.SetNextConverter(audioToDisplayConverter);
        }
    }
}
