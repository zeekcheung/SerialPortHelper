using System;

namespace SerialPortHelper.DataCheckers
{
    public class Crc8 : IDataChecker
    {
        /// <summary>
        /// 生成多项式
        /// </summary>
        private const byte Poly = 0x07;

        /// <summary>
        /// 初始值
        /// </summary>
        private const byte Init = 0x00;

        private static readonly byte[] s_table = BuildTable();

        private static byte[] BuildTable()
        {
            var t = new byte[256];
            for (var i = 0; i < 256; i++)
            {
                var crc = (byte)i;
                for (var j = 0; j < 8; j++)
                {
                    crc = (crc & 0x80) != 0 ? (byte)((crc << 1) ^ Poly) : (byte)(crc << 1);
                    t[i] = crc;
                }
            }
            return t;
        }

        public static byte Compute(ReadOnlySpan<byte> data)
        {
            var crc = Init;
            foreach (var b in data)
            {
                crc = s_table[crc ^ b];
            }
            return crc;
        }

        public bool Check(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
