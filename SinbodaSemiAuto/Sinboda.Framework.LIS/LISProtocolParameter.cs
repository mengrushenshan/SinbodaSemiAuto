using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.LIS
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class LISProtocolParameter
    {
        /// <summary>
        /// 接收缓冲器大小
        /// </summary>
        public int ReceiveBufferSize { get; set; }
        /// <summary>
        /// 发送数据大小
        /// </summary>
        public int SendBufferSize { get; set; }
        /// <summary>
        /// 字符编码
        /// </summary>
        public Common.Encoding Encoding
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public LISProtocolParameter()
        {
            ReceiveBufferSize = 10240;
            SendBufferSize = 10240;
            Encoding = Common.Encoding.Default;
        }
    }
}
