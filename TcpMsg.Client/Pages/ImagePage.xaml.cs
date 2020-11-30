using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TcpMsg.Client.Pages
{
    public partial class ImagePage : Page
    {
        public ImagePage(BitmapImage bitmap)
        {
            InitializeComponent();
            image.Source = bitmap;
        }
    }
}
