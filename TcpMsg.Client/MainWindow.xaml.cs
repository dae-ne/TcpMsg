using Microsoft.Win32;
using System;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TcpMsg.Client.MsgEncoding;

namespace TcpMsg.Client
{
    public partial class MainWindow : Window
    {
        private const string IpAddress = "127.0.0.1";
        private const int Port = 11000;
        private Connection _connection;
        private ToSendConverter _toSendConverter;
        private ToDisplayConverter _toDisplayConverter;
        private bool _isSending = false;
        private object _currentMessage = null;

        public MainWindow()
        {
            InitializeComponent();
            //DataContext = _connection;
            SetEncodingChains();

            var mp = new MediaElement();
            mp.Source
            
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
                await _connection.SendDisconnectMessage();
                _connection.Disconnect();
            }
            catch { }
        }

        private void SelectPictureBt_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
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
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Audio Files(*.mp3, *.wav, *.acc, *wma) | *.mp3; *.wav; *.acc; *.wma";

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

                if (message != null)
                {
                    try
                    {
                        var convertedMessage = _toSendConverter.Convert(message);
                        await _connection.Send(convertedMessage);
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

            if (message.Length > 0)
            {
                var messageType = _toDisplayConverter.Convert(message, out object receivedData);
                _currentMessage = receivedData;

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

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save a message";

            if (_currentMessage.GetType() == typeof(string))
            {
                saveFileDialog.Filter = "txt file | *.txt";

                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveFileDialog.FileName, textBlock.Text);
                }
            }
            else if (_currentMessage.GetType() == typeof(BitmapImage))
            {
                saveFileDialog.Filter = "Jpeg Image | *.jpg | Bitmap Image | *.bmp | Png Image | *.png";

                if (saveFileDialog.ShowDialog() == true)
                {
                    var bitmap = (BitmapImage)_currentMessage;
                    var encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));

                    using (var stream = saveFileDialog.OpenFile())
                    {
                        encoder.Save(stream);
                    }
                }

            }
            else if (_currentMessage.GetType() == typeof(int))
            {

            }

            // If the file name is not an empty string open it for saving.
            //if (saveFileDialog1.FileName != "")
            //{
            //    // Saves the Image via a FileStream created by the OpenFile method.
            //    //FileStream fs = (FileStream)saveFileDialog1.OpenFile();
            //    // Saves the Image in the appropriate ImageFormat based upon the
            //    // File type selected in the dialog box.
            //    // NOTE that the FilterIndex property is one-based.
            //    //switch (saveFileDialog1.FilterIndex)
            //    //{
            //    //    case 1:
            //    //        this.button2.Image.Save(fs,
            //    //          System.Drawing.Imaging.ImageFormat.Jpeg);
            //    //        break;

            //    //    case 2:
            //    //        this.button2.Image.Save(fs,
            //    //          System.Drawing.Imaging.ImageFormat.Bmp);
            //    //        break;

            //    //    case 3:
            //    //        this.button2.Image.Save(fs,
            //    //          System.Drawing.Imaging.ImageFormat.Gif);
            //    //        break;
            //    //}


            //    //fs.Close();
            //}
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
    }
}
