using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Timers;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SerialPortHelper.Helpers;
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
        public partial int SelectedBaudRate { get; set; } = 9600;

        [ObservableProperty]
        public partial Parity SelectedParity { get; set; } = Parity.None;

        [ObservableProperty]
        public partial int SelectedDataBits { get; set; } = 8;

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
        public partial int WriteBufferSize { get; set; }

        [ObservableProperty]
        public partial int ReadBufferSize { get; set; }

        [ObservableProperty]
        public partial bool IsRtsEnable { get; set; } = false;

        [ObservableProperty]
        public partial bool IsDtrEnable { get; set; } = false;

        [ObservableProperty]
        public partial bool IsSerialPortOpen { get; set; }

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

        /// <summary>
        /// 串口实例
        /// </summary>
        private readonly SerialPort _serialPort = new();

        /// <summary>
        /// 自动发送数据定时器
        /// </summary>
        private readonly Timer _autoSendDataTimer = new();

        /// <summary>
        /// 发送缓冲区
        /// </summary>
        private readonly List<byte> _sendBuffer = [];

        /// <summary>
        /// 接收缓冲区
        /// </summary>
        private readonly List<byte> _receiveBuffer = [];

        static MainViewModel()
        {
            // 注册额外的编码
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            EncodingItems = Encoding.GetEncodings().Select(x => x.Name).ToList();
        }

        public MainViewModel()
        {
            IsSerialPortOpen = _serialPort.IsOpen;
            WriteBufferSize = _serialPort.WriteBufferSize;
            ReadBufferSize = _serialPort.ReadBufferSize;

            // 设置自动发送数据定时器
            _autoSendDataTimer.Interval = AutoSendDataDelay;
            _autoSendDataTimer.Elapsed += HandleAutoSendDataTimerElapsed;

            StatusText = "初始化完成";
        }

        /// <summary>
        /// 发送区文本框失去焦点时，自动将文本框内容添加到发送缓冲区，并启动自动发送数据定时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HandleTxtSendDataLostFocus(object? sender, RoutedEventArgs e)
        {
            // 手动进行编码转换
            var data = SendDataText.IsHexString(out _)
                ? SendDataText.ToBytes()
                : _serialPort.Encoding.GetBytes(SendDataText);

            // 将数据添加到发送缓冲区
            _sendBuffer.Clear();
            _sendBuffer.AddRange(data);

            // 启动自动发送数据定时器
            if (IsAutoSendData)
            {
                _autoSendDataTimer.Start();
            }
        }

        /// <summary>
        /// 自动发送数据定时器事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleAutoSendDataTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            // 发送数据
            SendData();

            // 停止定时器
            _autoSendDataTimer.Stop();
        }

        partial void OnIsHexSendDataChanged(bool value)
        {
            SendDataText = value
                ? _sendBuffer.ToHexString()
                : _serialPort.Encoding.GetString(_sendBuffer.ToArray()).Replace("\0", "\\0");
        }

        partial void OnIsHexReceiveDataChanged(bool value)
        {
            ReceivedDataText = value
                ? _receiveBuffer.ToHexString()
                : _serialPort.Encoding.GetString(_receiveBuffer.ToArray()).Replace("\0", "\\0");
        }

        partial void OnAutoSendDataDelayChanged(int value)
        {
            _autoSendDataTimer.Interval = value;
        }

        [RelayCommand]
        private void ToggleSerialPort()
        {
            // 打开或关闭串口
            if (_serialPort.IsOpen)
                CloseSerialPort();
            else
                OpenSerialPort();

            // 将界面中的串口打开状态设置为串口的实际打开状态
            IsSerialPortOpen = _serialPort.IsOpen;
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        private void OpenSerialPort()
        {
            try
            {
                // 设置串口参数
                _serialPort.PortName = SelectedSerialPortName;
                _serialPort.BaudRate = SelectedBaudRate;
                _serialPort.Parity = SelectedParity;
                _serialPort.DataBits = SelectedDataBits;
                _serialPort.StopBits = SelectedStopBits;
                _serialPort.Encoding = Encoding.GetEncoding(SelectedEncoding);
                _serialPort.Handshake = SelectedHandshake;
                _serialPort.ReadTimeout = ReceiveTimeout;
                _serialPort.WriteTimeout = SendTimeout;
                _serialPort.ReceivedBytesThreshold = ReceivedBytesThreshold;
                _serialPort.RtsEnable = IsRtsEnable;
                _serialPort.DtrEnable = IsDtrEnable;

                // 打开串口
                _serialPort.Open();

                // 接收数据
                _serialPort.DataReceived += HandleDataReceived;

                StatusText = $"串口 {_serialPort.PortName} 已打开";
            }
            catch (Exception ex)
            {
                MessageBox.ShowAsync(
                    $"打开串口失败：{ex.Message}",
                    icon: MessageBoxIcon.Error,
                    button: MessageBoxButton.OK
                );
            }
        }

        /// <summary>
        /// 处理接收数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // 是否继续接收数据
            if (!IsReceiveData)
                return;

            // 读取串口接收缓冲区中的字节数据
            var buffer = new byte[_serialPort.BytesToRead];
            _serialPort.Read(buffer, 0, buffer.Length);

            StatusText = $"已接收 {buffer.Length} 字节数据";

            // 统计接收数据数量
            ReceivedDataCount += buffer.Length;

            // 自动清除接收数据
            if (IsAutoClearReceiveData)
            {
                // 如果接收到的字节数超过自动清除阈值，则先清除接收数据
                if (_receiveBuffer.Count + buffer.Length > AutoClearReceiveDataThreshold)
                {
                    ClearReceiveData();
                }

                // 如果接收到的字节数仍然超过自动清除阈值，则不添加到接收缓冲区
                if (buffer.Length > AutoClearReceiveDataThreshold)
                {
                    return;
                }
            }

            // 将接收到的字节数据添加到接收缓冲区
            _receiveBuffer.AddRange(buffer);

            // 将字节数据解码为十六进制字符串或对应编码字符串
            var data = IsHexReceiveData
                ? buffer.ToHexString()
                : _serialPort.Encoding.GetString(buffer).Replace("\0", "\\0");

            // 将接收到的数据添加到接收区
            ReceivedDataText += data;
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        private void CloseSerialPort()
        {
            try
            {
                _serialPort.Close();

                StatusText = $"串口 {_serialPort.PortName} 已关闭";
            }
            catch (Exception ex)
            {
                MessageBox.ShowAsync(
                    $"关闭串口失败：{ex.Message}",
                    icon: MessageBoxIcon.Error,
                    button: MessageBoxButton.OK
                );
            }
        }

        [RelayCommand]
        private void SendData()
        {
            try
            {
                // 发送字符串数据，SerialPort 会自动进行编码转换
                // s_serialPort.Write(SendDataText);

                // 手动进行编码转换，并发送字节数据，SerialPort 不会对字节数据进行编码转换
                _serialPort.Write(_sendBuffer.ToArray(), 0, _sendBuffer.Count);

                // 统计发送数据数量
                SendDataCount += _sendBuffer.Count;

                StatusText = $"已发送 {_sendBuffer.Count} 字节数据";
            }
            catch (Exception ex)
            {
                MessageBox.ShowAsync(
                    $"发送数据失败：{ex.Message}",
                    icon: MessageBoxIcon.Error,
                    button: MessageBoxButton.OK
                );
            }
        }

        [RelayCommand]
        private void ClearSendData()
        {
            SendDataText = string.Empty;
            _sendBuffer.Clear();

            StatusText = "发送数据已清除";
        }

        [RelayCommand]
        private void SendFile() { }

        [RelayCommand]
        private void ClearReceiveData()
        {
            ReceivedDataText = string.Empty;
            _receiveBuffer.Clear();

            StatusText = "接收数据已清除";
        }

        [RelayCommand]
        private void SaveReceivedData() { }

        [RelayCommand]
        private void ResetStatus()
        {
            SendDataCount = 0;
            ReceivedDataCount = 0;
            ClearSendData();
            ClearReceiveData();

            StatusText = "状态已重置";
        }
    }
}
