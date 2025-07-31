namespace SerialPortHelper.DataCheckers
{
    public class Crc16Checker : IDataChecker
    {
        public bool Check(byte[] frame)
        {
            // 计算所得的校验码（帧头和长度也参与运算）
            var cacl = Crc16CcittFalse(frame[..^2], bigEndian: true);
            // 接收所得的校验码
            var recv = (ushort)((frame[^2] << 8) | frame[^1]);

            // 判断两个校验码是否相同
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
            const ushort Poly = 0x1021;
            ushort crc = 0xFFFF;

            foreach (var b in data)
            {
                crc ^= (ushort)(b << 8);

                for (var i = 0; i < 8; i++)
                {
                    crc = (crc & 0x8000) != 0 ? (ushort)((crc << 1) ^ Poly) : (ushort)(crc << 1);
                }
            }

            return bigEndian ? crc : (ushort)((crc << 8) | (crc >> 8));
        }
    }
}
