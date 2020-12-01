using Microsoft.Win32;

namespace TcpMsg.Client.FileIO
{
    class Reader
    {
        private string _dialogFilter;

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
