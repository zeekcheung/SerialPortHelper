using Avalonia.Controls;
using SerialPortHelper.ViewModels;

namespace SerialPortHelper.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
        }
    }
}
