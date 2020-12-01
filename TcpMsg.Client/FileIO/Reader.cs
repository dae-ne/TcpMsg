using Microsoft.Win32;
using System;

namespace TcpMsg.Client.FileIO
{
    class Reader
    {
        private string _dialogFilter;

        public Reader(DataType type)
        {
            SetDataType(type);
        }

        public void SetDataType(DataType type)
        {
            switch (type)
            {
                case DataType.Text:
                    _dialogFilter = "txt file | *.txt";
                    break;

                case DataType.Image:
                    _dialogFilter = "Bitmap Image | *.bmp";
                    break;

                case DataType.Audio:
                    _dialogFilter = "Waveform | *.wav";
                    break;

                default:
                    throw new Exception("Wrong data type");
            }
        }

        public string GetUriDialog()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = _dialogFilter
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            return "";
        }
    }
}
