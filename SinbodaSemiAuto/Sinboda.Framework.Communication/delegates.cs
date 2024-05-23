using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Communication
{
    /// <summary>
    /// 接收数据委托
    /// </summary>
    public delegate void ClientDataReceivedEventHandler(object sender, ReceivedEventArgs e);
    /// <summary>
    /// 通讯错误委托
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void CommunicateErrorEventHandler(object sender, CommunicateErrorEventArgs e);
}
