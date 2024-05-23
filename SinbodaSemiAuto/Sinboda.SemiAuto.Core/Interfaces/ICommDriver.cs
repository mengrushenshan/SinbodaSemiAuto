using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Interfaces
{
    /// <summary>
    /// 通讯器
    /// </summary>
    public interface ICommDriver : IDisposable
    {
        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        bool Connect();

        /// <summary>
        /// 连接状态
        /// </summary>
        /// <returns></returns>
        bool IsConnected();

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        bool Write(byte[] bytes);

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <returns></returns>
        byte[] Read();
    }
}
