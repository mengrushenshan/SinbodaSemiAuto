using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.LIS.SinHL7
{
    /// <summary>
    /// 网口通讯参数
    /// </summary>
    public class NetworkParameter : LISProtocolParameter
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public NetworkParameter() : base()
        {
            RemoteAddress = "127.0.0.1";
            RemotePort = 2017;
            Timeout = 3000;
        }
        /// <summary>
        /// IP
        /// </summary>
        public string RemoteAddress { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int RemotePort { get; set; }
        /// <summary>
        /// 网口：连接超时，单位毫秒，默认3秒
        /// </summary>
        public int Timeout
        { get; set; }
    }
}
