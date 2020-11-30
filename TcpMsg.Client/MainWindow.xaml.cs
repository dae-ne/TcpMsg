using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TcpMsg.Client.Components;
using TcpMsg.Client.MsgEncoding;
using TcpMsg.Client.Pages;

namespace TcpMsg.Client
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const string IpAddress = "127.0.0.1";
        private const int Port = 11000;
        private readonly Connection _connection;
        private ToSendConverter _toSendConverter;
        private ToDisplayConverter _toDisplayConverter;
        private bool _isSending = false;
        private object _currentMessage = null;

        private string myVar;

        public event PropertyChangedEventHandler PropertyChanged;

        public string MyProperty
        {
            get { return myVar; }
            set
            {
                myVar = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MyProperty)));
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            //DataContext = _connection;
            SetEncodingChains();
            
            try
            {
                _connection = new Connection(IpAddress, Port);
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
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Bitmap Image | *.bmp"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ImgUriTextBlock.Text = openFileDialog.FileName;
            }

            AudioUriTextBlock.Text = "";
            MsgTextBox.Text = "";
        }

        private void SelectAudioFileBt_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Waveform | *.wav"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                AudioUriTextBlock.Text = openFileDialog.FileName;
            }

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
                        var convertedMessage = _toSendConverter.Convert(message);
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
            MyProperty = MyProperty + "a";
            //var message = _connection.NextMessage();

            //if (message.Length > 0)
            //{
            //    var messageType = _toDisplayConverter.Convert(message, out object receivedData);
            //    _currentMessage = receivedData;

            //    if (messageType == typeof(string))
            //    {
            //        Main.Content = new TextPage(receivedData as string);
            //    }
            //    else if (messageType == typeof(BitmapImage))
            //    {
            //        Main.Content = new ImagePage(receivedData as BitmapImage);
            //    }
            //    else if (messageType == typeof(Audio))
            //    {
            //        Main.Content = new AudioPage(receivedData as Audio);
            //    }
            //}
        }

        private async void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Save a message"
            };

            if (_currentMessage.GetType() == typeof(string))
            {
                saveFileDialog.Filter = "txt file | *.txt";

                if (saveFileDialog.ShowDialog() == true)
                {
                    await File.WriteAllTextAsync(saveFileDialog.FileName, _currentMessage as string);
                }
            }
            else if (_currentMessage.GetType() == typeof(BitmapImage))
            {
                saveFileDialog.Filter = "Bitmap Image | *.bmp";

                if (saveFileDialog.ShowDialog() == true)
                {
                    var bitmap = (BitmapImage)_currentMessage;
                    var encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    using var stream = saveFileDialog.OpenFile();
                    encoder.Save(stream);
                }

            }
            else if (_currentMessage.GetType() == typeof(int))
            {

            }
        }

        private void SetEncodingChains()
        {
            _toSendConverter = new TextToSendConverter();
            var imgToSendConverter = new ImageToSendConverter();
            var audioToSendConverter = new AudioToSendConverter();
            _toSendConverter.SetNextConverter(imgToSendConverter);
            imgToSendConverter.SetNextConverter(audioToSendConverter);

            _toDisplayConverter = new TextToDisplayConverter();
            var imgToDisplayConverter = new ImageToDisplayConverter();
            var audioToDisplayConverter = new AudioToDisplayConverter();
            _toDisplayConverter.SetNextConverter(imgToDisplayConverter);
            imgToDisplayConverter.SetNextConverter(audioToDisplayConverter);
        }
    }
}
