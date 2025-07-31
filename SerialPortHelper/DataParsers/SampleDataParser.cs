using System.Collections.Generic;
using System.Linq;

namespace SerialPortHelper.DataParsers
{
    /// <summary>
    /// 示例数据解析器
    /// 数据格式：帧头（0x7F）+ 数据长度（1字节）+ 数据（N字节）+ 校验码（1字节/CRC8）
    /// </summary>
    public class SampleDataParser : IDataParser
    {
        /// <summary>
        /// 帧头
        /// </summary>
        private const byte FrameHeader = 0x7F;

        /// <summary>
        /// 最小帧长度：帧头（1字节）+ 数据长度（1字节）+ 校验码（2字节）
        /// </summary>
        private const int MinFrameLength = 4;

        /// <summary>
        /// 数据队列
        /// </summary>
        private readonly Queue<byte> _queue = new();

        /// <summary>
        /// 迭代器函数，循环解析所有数据帧
        /// </summary>
        /// <param name="buffer">数据缓冲区</param>
        /// <returns>数据帧迭代器</returns>
        public IEnumerable<byte[]> GetDataFrames(byte[] buffer)
        {
            // 所有字节入队
            foreach (var b in buffer)
            {
                _queue.Enqueue(b);
            }

            // 解析所有数据帧
            while (true)
            {
                // 找帧头
                while (_queue.Count > 0 && _queue.Peek() != FrameHeader)
                {
                    _queue.Dequeue();
                }

                // 判断是否有最小帧，确保数据长度和校验码字段存在
                if (_queue.Count < MinFrameLength)
                {
                    // 终止整个迭代器，跳出循环
                    yield break;
                }

                // 读取数据长度，确定数据帧长度
                var payloadLength = _queue.ElementAt(1);
                var frameLength = MinFrameLength + payloadLength;

                // 判断是否有足够的数据，确保数据帧完整
                if (_queue.Count < frameLength)
                {
                    // 终止整个迭代器，跳出循环
                    yield break;
                }

                // 读取实际数据，组装成数据帧
                var frame = new byte[frameLength];
                for (var i = 0; i < frameLength; i++)
                {
                    frame[i] = _queue.Dequeue();
                }

                // 返回数据帧，交出控制权
                yield return frame;
            }
        }
    }
}
