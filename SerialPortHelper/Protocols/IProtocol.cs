using SerialPortHelper.DataCheckers;
using SerialPortHelper.DataParsers;

namespace SerialPortHelper.Protocols
{
    public interface IProtocol
    {
        /// <summary>
        /// 数据解析器
        /// </summary>
        IDataParser Parser { get; set; }

        /// <summary>
        /// 数据校验器
        /// </summary>
        IDataChecker Checker { get; set; }

        /// <summary>
        /// 解析所有数据帧并校验
        /// </summary>
        /// <param name="buffer">数据缓冲区</param>
        /// <param name="handler">数据帧处理器</param>
        void ParseAndCheck(byte[] buffer, FrameHandler handler);
    }

    public delegate void FrameHandler(byte[] frame, bool isValid);
}
