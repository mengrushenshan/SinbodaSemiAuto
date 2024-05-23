using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Helpers
{
    public static class BitHelper
    {
        //获取字节任意位的值 1 or 0
        public static int GetBitByIndex(byte value, int index)
        {
            if (index > 7)
            {
                throw new Exception();
            }
            return (value & (0x1 << index)) >> index;
        }
        //获取指定起点和终点连续位表示的int值, startIndex = 0, endIndex = values.Length * 8 - 1
        //限制截取长度31位
        public static int GetIntFromPart(byte[] values, int startIndex, int endIndex)
        {
            int result = 0;
            for (int i = startIndex; i <= endIndex; i++)
            {
                //int n = values.Length - (i / 8) - 1;
                int n = i / 8;
                int p = i > 7 ? i % 8 : i;
                int z = GetBitByIndex(values[n], p);
                result += GetBitByIndex(values[n], p) << (i - startIndex);
            }
            return result;
        }
        /// <summary>
        /// byte高位在前，int i = data.Length - 1, j = 0; i >= 0; i--, j++
        /// byte高位在后
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startBit"></param>
        /// <param name="bitLength"></param>
        /// <returns></returns>
        public static ulong GetMotorolaSignalValue(byte[] data, int startBit, int bitLength)
        {
            ulong canSignalValue = 0;
            for (int i = data.Length - 1, j = 0; i >= 0; i--, j++)
            {
                canSignalValue += (ulong)data[j] << i * 8;
            }
            int x = startBit / 8;
            int y = startBit % 8;
            //int z = x * 8 + bitLength - y;
            //int rightMoveCount = data.Length * 8 - z;
            //canSignalValue >>= rightMoveCount;
            //canSignalValue = canSignalValue & ulong.MaxValue >> 64 - bitLength;
            int datahigh = x * 8 + 8 - y - bitLength;
            canSignalValue <<= datahigh;//取最高位
            int rightMoveCount = data.Length * 8 - bitLength;
            canSignalValue >>= rightMoveCount;
            canSignalValue = canSignalValue & ulong.MaxValue >> 64 - bitLength;
            return canSignalValue;
        }
    }
}
