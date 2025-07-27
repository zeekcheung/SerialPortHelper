using Avalonia.Controls;
using Avalonia.Interactivity;
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

        /// <summary>
        /// 发送区文本框失去焦点时，自动将文本框内容添加到发送缓冲区，并启动自动发送数据定时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTxtSendDataLostFocus(object? sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.HandleTxtSendDataLostFocus(sender, e);
            }
        }
    }
}
