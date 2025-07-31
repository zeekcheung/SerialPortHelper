using System.Collections.Generic;

namespace SerialPortHelper.DataParsers
{
    /// <summary>
    /// 数据解析器接口
    /// </summary>
    public interface IDataParser
    {
        /// <summary>
        /// 解析原始数据，提取数据帧
        /// </summary>
        /// <param name="buffer">数据缓冲区</param>
        /// <returns>数据帧可迭代对象</returns>
        IEnumerable<byte[]> GetDataFrames(byte[] buffer);
    }
}
