using Microsoft.Win32;
using System;
using System.Threading.Tasks;

namespace TcpMsg.Client.FileIO
{
    abstract class Writer
    {
        protected Type _type;
        protected string _filter;

        public Writer(Type type, string filter)
        {
            _type = type;
            _filter = filter;
        }

        public async Task SaveToFile(object data)
        {
            if (_type == data.GetType())
            {
                var saveFileDialog = CreateSaveDialog();
                SetFilter(saveFileDialog);
                await SaveData(saveFileDialog, data);
            }
        }

        protected SaveFileDialog CreateSaveDialog()
        {
            return new SaveFileDialog
            {
                Title = "Save a message"
            };
        }
        
        public void SetFilter(SaveFileDialog saveFileDialog)
        {
            saveFileDialog.Filter = _filter;
        }

        protected abstract Task SaveData(SaveFileDialog saveFileDialog, object data);
    }
}
