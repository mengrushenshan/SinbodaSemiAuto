using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISubSystem : IDisposable
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="driver"></param>
        void Init(ICommDriver driver);

        /// <summary>
        /// 开始执行序列
        /// </summary>
        void StartSequence();

        /// <summary>
        /// 暂停执行序列
        /// </summary>
        void PauseSequence();

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
        /// 发送数据（异步
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        bool SendAsync(IDataFrame frame);

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        bool Send(IDataFrame frame);

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <returns></returns>
        IDataFrame Receive();

    }
}
