using SerialPortHelper.DataCheckers;
using SerialPortHelper.DataParsers;

namespace SerialPortHelper.Protocols
{
    public class SampleProtocol : IProtocol
    {
        public IDataParser Parser { get; set; } = new SampleDataParser();
        public IDataChecker Checker { get; set; } = new Crc16Checker();

        public void ParseAndCheck(byte[] buffer, FrameHandler handler)
        {
            // 提取所有数据帧
            var frames = Parser.GetDataFrames(buffer);

            // 提取每个帧的实际数据并进行数据校验
            foreach (var frame in frames)
            {
                // 校验数据
                var isValid = Checker.Check(frame);

                handler?.Invoke(frame, isValid);
            }
        }
    }
}
