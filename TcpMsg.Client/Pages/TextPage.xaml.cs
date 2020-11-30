using System.Windows.Controls;

namespace TcpMsg.Client.Pages
{
    public partial class TextPage : Page
    {
        public TextPage(string text)
        {
            InitializeComponent();
            textBlock.Text = text;
        }
    }
}
