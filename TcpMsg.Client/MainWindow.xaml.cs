using Microsoft.Win32;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace TcpMsg.Client
{
    public partial class MainWindow : Window
    {
        private const string DisconnectMessage = "<<disconnectme>>";
        private TcpClient _client = null;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                _client = new TcpClient("127.0.0.1", 11000);
                this.Closed += new EventHandler(MainWindow_Closed);
                _ = StartListeningAsync();
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\n\n" +
                    $"Check if the server is running.\n" +
                    $"Application will be closed.", e.GetType().Name);
                Application.Current.Shutdown();
            }
        }

        private async Task StartListeningAsync()
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
                            MessageBox.Show("There is a problem with a recived message.");
                        }

                        using var ms = new MemoryStream(data);
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = ms;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        image.Source = bitmap;
                    }
                }
                catch
                {
                    Application.Current.Shutdown();
                }
                
                await Task.Delay(300);
            }
        }

        private async void MainWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                await SendMessage(DisconnectMessage, false);
                _client.GetStream().Close();
                _client.Close();
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
            if (MsgTextBox.Text != "")
            {
                await SendMessage(MsgTextBox.Text);
                MsgTextBox.Text = "";
            }
            else if (ImgUriTextBlock.Text != "")
            {
                var bitmapImage = new BitmapImage(new Uri(ImgUriTextBlock.Text));
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                using MemoryStream ms = new MemoryStream();
                encoder.Save(ms);
                var data = ms.ToArray();
                await SendBytes(data);
            }

            await SendMessage(MsgTextBox.Text);
            MsgTextBox.Text = "";
        }

        private async Task SendMessage(string message, bool withSize = true)
        {
            var data = Encoding.UTF8.GetBytes(message);
            await SendBytes(data, withSize);
        }

        private async Task SendBytes(byte[] data, bool withSize = true)
        {
            try
            {
                var stream = _client.GetStream();

                if (withSize)
                {
                    var streamSize = BitConverter.GetBytes(data.Length);
                    await stream.WriteAsync(streamSize, 0, streamSize.Length);
                }

                await stream.WriteAsync(data, 0, data.Length);
            }
            catch
            {
                Application.Current.Shutdown();
            }
        }
    }
}
