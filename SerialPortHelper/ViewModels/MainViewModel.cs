using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ursa.Controls;

namespace SerialPortHelper.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// 预设的串口名
        /// </summary>
        public static IReadOnlyList<string> SerialPortNameItems { get; } =
            SerialPort.GetPortNames();

        /// <summary>
        /// 预设的波特率
        /// </summary>
        public static IReadOnlyList<int> BaudRatesItems { get; } = [9600, 57600, 115200];

        /// <summary>
        /// 预设的校验位
        /// </summary>
        public static IReadOnlyList<Parity> ParityItems { get; } = Enum.GetValues<Parity>();

        /// <summary>
        /// 预设的数据位
        /// </summary>
        public static IReadOnlyList<int> DataBitsItems { get; set; } = [5, 6, 7, 8];

        /// <summary>
        /// 预设的停止位
        /// </summary>
        public static IReadOnlyList<StopBits> StopBitsItems { get; set; } =
            Enum.GetValues<StopBits>();

        /// <summary>
        /// 预设的编码
        /// </summary>
        public static IReadOnlyList<string> EncodingItems { get; set; }

        /// <summary>
        /// 预设的握手方式
        /// </summary>
        public static IReadOnlyList<Handshake> HandshakeItems { get; set; } =
            Enum.GetValues<Handshake>();

        [ObservableProperty]
        public partial string SelectedSerialPortName { get; set; } =
            SerialPortNameItems.Count > 0 ? SerialPortNameItems[0] : "";

        [ObservableProperty]
        public partial int SelectedBaudRate { get; set; } = BaudRatesItems[0];

        [ObservableProperty]
        public partial Parity SelectedParity { get; set; } = Parity.None;

        [ObservableProperty]
        public partial int SelectedDataBits { get; set; } = DataBitsItems[0];

        [ObservableProperty]
        public partial StopBits SelectedStopBits { get; set; } = StopBits.One;

        [ObservableProperty]
        public partial string SelectedEncoding { get; set; } = "gb2312";

        [ObservableProperty]
        public partial Handshake SelectedHandshake { get; set; } = Handshake.None;

        [ObservableProperty]
        public partial int SendTimeout { get; set; } = SerialPort.InfiniteTimeout;

        [ObservableProperty]
        public partial int ReceiveTimeout { get; set; } = SerialPort.InfiniteTimeout;

        [ObservableProperty]
        public partial int ReceivedBytesThreshold { get; set; } = 1;

        [ObservableProperty]
        public partial int WriteBufferSize { get; set; } = 2048;

        [ObservableProperty]
        public partial int ReadBufferSize { get; set; } = 4096;

        [ObservableProperty]
        public partial bool IsRtsEnable { get; set; } = false;

        [ObservableProperty]
        public partial bool IsDtrEnable { get; set; } = false;

        [ObservableProperty]
        public partial bool IsSerialPortOpen { get; set; } = false;

        [ObservableProperty]
        public partial bool IsAutoClearReceiveData { get; set; } = false;

        [ObservableProperty]
        public partial int AutoClearReceiveDataThreshold { get; set; } = 1024;

        [ObservableProperty]
        public partial bool IsHexReceiveData { get; set; } = false;

        [ObservableProperty]
        public partial bool IsReceiveData { get; set; } = true;

        [ObservableProperty]
        public partial bool IsAutoSendData { get; set; } = false;

        [ObservableProperty]
        public partial int AutoSendDataDelay { get; set; } = 1000;

        [ObservableProperty]
        public partial bool IsHexSendData { get; set; } = false;

        [ObservableProperty]
        public partial string SelectedSaveFilePath { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string ReceivedDataText { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string SendDataText { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string StatusText { get; set; } = string.Empty;

        [ObservableProperty]
        public partial int SendDataCount { get; set; } = 0;

        [ObservableProperty]
        public partial int ReceivedDataCount { get; set; } = 0;

        [ObservableProperty]
        public partial string SelectedSendFilePath { get; set; } = string.Empty;

        static MainViewModel()
        {
            // 注册额外的编码
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            EncodingItems = Encoding.GetEncodings().Select(x => x.Name).ToList();
        }

        public MainViewModel()
        {
            StatusText = "初始化完成";
        }

        [RelayCommand]
        private void ToggleSerialPort()
        {
            MessageBox.ShowAsync("切换状态");

            if (IsSerialPortOpen)
                CloseSerialPort();
            else
                OpenSerialPort();

            // TODO: 手动更新串口打开状态
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        private void OpenSerialPort() { }

        /// <summary>
        /// 关闭串口
        /// </summary>
        private static void CloseSerialPort() { }

        [RelayCommand]
        private void SendData() { }

        [RelayCommand]
        private void ClearSendData() { }

        [RelayCommand]
        private void SendFile() { }

        [RelayCommand]
        private void ClearReceiveData() { }

        [RelayCommand]
        private void SaveReceivedData() { }

        [RelayCommand]
        private void ResetStatus() { }
    }
}
