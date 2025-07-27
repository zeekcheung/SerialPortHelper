using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SerialPortHelper.Helpers
{
    public static class ConvertExtensions
    {
        /// <summary>
        /// 将字节集合转换为十六进制字符串
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="separator">分隔符</param>
        /// <returns>十六进制字符串</returns>
        public static string ToHexString(this IEnumerable<byte> bytes, string separator = " ")
        {
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("X2"));
                sb.Append(separator);
            }

            if (sb.Length > 0)
            {
                sb.Length -= separator.Length;
            }

            return sb.ToString();
        }

        /// <summary>
        /// 将十六进制字符串转换为字节数组
        /// </summary>
        /// <param name="hexString">十六进制字符串</param>
        /// <returns>字节数组</returns>
        /// <exception cref="FormatException">字符串不是有效的十六进制字符串</exception>
        public static byte[] ToBytes(this string hexString)
        {
            if (!IsHexString(hexString, out var countOfHexChar))
            {
                throw new FormatException("字符串不是有效的十六进制字符串");
            }

            // 去除前后缀
            var span = hexString.AsSpan().Trim();
            span = span.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? span[2..] : span;

            // 分配字节数组，每个字节占 8 位，每个十六进制字符占 4 位，每两个十六进制字符组成一个字节
            var bytes = new byte[countOfHexChar >> 1];

            // 遍历字符串，将每两个十六进制字符组成一个字节
            for (int i = 0, idx = 0; idx < bytes.Length; idx++)
            {
                // 左侧为高位，右侧为低位
                var hi = NextNoneWhiteSpace(span, ref i);
                var lo = NextNoneWhiteSpace(span, ref i);

                // 将高位和低位依次转化为十进制、二进制，再组合成字节
                bytes[idx] = (byte)((ParseNibble(hi) << 4) | ParseNibble(lo));
            }

            return bytes;
        }

        /// <summary>
        /// 获取字符串中下一个非空白字符并后移位置
        /// </summary>
        /// <param name="span">字符串</param>
        /// <param name="pos">当前位置</param>
        /// <returns>下一个非空白字符</returns>
        /// <exception cref="FormatException">无法找到下一个非空白字符</exception>
        private static char NextNoneWhiteSpace(ReadOnlySpan<char> span, ref int pos)
        {
            // 跳过所有空白字符
            while (pos < span.Length && char.IsWhiteSpace(span[pos]))
            {
                pos++;
            }

            // 判断是否越界
            if (pos >= span.Length)
            {
                throw new FormatException($"无法找到下一个非空白字符，{pos} 已经越界");
            }

            // 返回下一个有效字符，并后移位置
            return span[pos++];
        }

        /// <summary>
        /// 将单个十六进制字符转换为十进制整数
        /// </summary>
        /// <param name="c">十六进制字符</param>
        /// <returns>十进制整数</returns>
        /// <exception cref="FormatException">字符不是有效的十六进制字符</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ParseNibble(this char c) =>
            c switch
            {
                >= '0' and <= '9' => c - '0',
                >= 'a' and <= 'f' => c - 'a' + 10,
                >= 'A' and <= 'F' => c - 'A' + 10,
                _ => throw new FormatException($"'{c}' 不是有效的十六进制字符"),
            };

        /// <summary>
        /// 判断字符是否为十六进制字符
        /// </summary>
        /// <param name="c">字符</param>
        /// <returns>字符s是否为十六进制字符</returns>
        public static bool IsHexChar(this char c) =>
            c is >= '0' and <= '9' or >= 'a' and <= 'f' or >= 'A' and <= 'F';

        /// <summary>
        /// 判断字符串是否为有效的十六进制字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="countOfHexChar">有效的十六进制字符个数</param>
        /// <returns>字符串是否是有效的十六进制字符串</returns>
        public static bool IsHexString(this string str, out int countOfHexChar)
        {
            countOfHexChar = 0;

            // 不能为空
            if (string.IsNullOrWhiteSpace(str))
            {
                return false;
            }

            // 去除前后缀
            var span = str.AsSpan().Trim();
            span = span.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? span[2..] : span;

            // 除了空格之外，不能包含其他非十六进制字符
            foreach (var c in span)
            {
                if (char.IsWhiteSpace(c))
                {
                    continue;
                }

                if (!IsHexChar(c))
                {
                    return false;
                }

                countOfHexChar++;
            }

            // 十六进制字符个数必须为偶数
            return countOfHexChar > 0 && (countOfHexChar & 1) == 0;
        }
    }
}
