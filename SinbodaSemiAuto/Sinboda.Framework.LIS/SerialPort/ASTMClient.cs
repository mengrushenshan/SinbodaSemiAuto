using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.LIS.SinASTM
{
    public class ASTMClient : ASTMSerialPort
    {

        /// <summary>
        /// 发送超时
        /// </summary>
        public int Timeout
        {
            get
            {
                return timeout;
            }
            set
            {
                timeout = value;
            }

        }
        /// <summary>
        /// 超时重发
        /// </summary>
        public int ReSendTime
        {
            get
            {
                return reSendTime;
            }
            set
            {
                reSendTime = value;
            }

        }
        public ASTMClient()
        {
        }



        /// <summary>
        /// 发送ASTM Message
        /// </summary>
        //public void SendASTM()
        //{
        //    base.SendASTM()
        //}



        //protected override void OnDisconnectEvent(SerialPort workPort)
        //{
        //    if (OnDisconnect != null)
        //    {
        //        OnDisconnect(this);
        //    }
        //}
        /// <summary>
        /// 析构
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            //Disconnect();
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        //public void Close()
        //{
        //    try
        //    {
        //        Disconnect();
        //        OnDisconnectEvent(null);
        //    }
        //    catch (Exception e)
        //    {
        //        OnErrorEvent(new ErrorEventArgs(e));
        //    }
        //}
        //public event ConnectEvent OnConnect;
        //public event DisconnectEvent OnDisconnect;
    }
}
