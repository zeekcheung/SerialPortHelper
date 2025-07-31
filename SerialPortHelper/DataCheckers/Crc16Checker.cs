namespace SerialPortHelper.DataCheckers
{
    public class Crc16Checker : IDataChecker
    {
        public bool Check(byte[] frame)
        {
            var cacl = Crc16CcittFalse(frame[..^2], bigEndian: true);
            var recv = (ushort)((frame[^2] << 8) | frame[^1]);

            return cacl == recv;
        }

        /// <summary>
        /// 计算校验码
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="bigEndian">是否大端模式</param>
        /// <returns>校验码</returns>
        public static ushort Crc16CcittFalse(byte[] data, bool bigEndian = true)
        {
            ushort crc = 0xFFFF;
            foreach (var b in data)
            {
                var val = bigEndian ? (ushort)(b << 8) : b;
                crc ^= val;
                for (var i = 0; i < 8; i++)
                {
                    crc = (crc & 0x8000) != 0 ? (ushort)((crc << 1) ^ 0x1021) : (ushort)(crc << 1);
                }
            }
            return crc;
        }
    }
}
