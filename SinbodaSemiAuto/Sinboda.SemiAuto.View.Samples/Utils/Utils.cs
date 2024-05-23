using Sinboda.Framework.Control.ItemSelection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.View.Samples.Utils
{
    internal class Utils
    {
        /// <summary>
        /// 返回样本号范围
        /// </summary>
        /// <param name="value">样本号范围文本</param>
        /// <returns>返回 <seealso cref="List{Int32}"/>类型，<seealso cref="List{T}.Count"/> 为 0 时格式错误</returns>
        public static List<int> GetSampleCodeRange(string value)
        {
            var result = new List<int>();
            if (string.IsNullOrEmpty(value))
                return result;
            value = value.TrimEnd('-');
            if (string.IsNullOrEmpty(value))
                return result;

            string[] containComma = value.Split('-');
            if (containComma.Length == 2)
            {
                int v1, v2 = 0;
                if (!int.TryParse(containComma[0], out v1) || !int.TryParse(containComma[1], out v2))
                    return result;

                int min = v1 >= v2 ? v2 : v1;
                int max = v2 >= v1 ? v2 : v1;
                result.Add(min);
                result.Add(max);
            }
            else
            {
                int code = 0;
                if (!int.TryParse(value, out code))
                    return result;

                result.Add(code);
                result.Add(code);
            }
            return result;
        }
    }
}
