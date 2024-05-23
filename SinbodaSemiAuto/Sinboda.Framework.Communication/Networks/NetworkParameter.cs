using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Communication.Networks
{
    /// <summary>
    /// 网口通讯参数
    /// </summary>
    public class NetworkParameter : ProtocolParameter
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public NetworkParameter() : base()
        {
            RemoteAddress = "127.0.0.1";
            RemotePort = 2017;
        }
        /// <summary>
        /// IP
        /// </summary>
        public string RemoteAddress { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int RemotePort { get; set; }
    }
}
