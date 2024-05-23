using Sinboda.Framework.Common.Log;
using Sinboda.Framework.LIS.Network;
using Sinboda.Framework.LIS.SinASTM;
using Sinboda.Framework.LIS.SinHL7;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sinboda.Framework.LIS
{
    /// <summary>
    /// LIS通讯处理工厂类
    /// </summary>
    public class LISProtocolFactoryHandle
    {
        private static readonly LISProtocolFactoryHandle instance = new LISProtocolFactoryHandle();
        /// <summary>
        /// 
        /// </summary>
        public static LISProtocolFactoryHandle Instance
        {
            get
            {
                return instance;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private LISProtocolFactoryHandle()
        {

        }

        public ManualResetEvent autoResetEvent = new ManualResetEvent(false);

        private ClientLISProtocol _protocol = null;
        /// <summary>
        /// 根据协议参数,实例化客户端模式设备协议
        /// </summary>
        /// <param name="p_paramter">协议参数</param>
        /// <returns></returns>
        public ClientLISProtocol GetClientInstanceProtocol(LISProtocolParameter p_paramter)
        {
            //串口
            if (p_paramter is SerialPortParameter)
            {
                _protocol = new SerialPortClientLISProtocol();
            }
            else
            {
                if (_protocol != null)
                {
                    LogHelper.logLisComm.Info("【LIS底层】GetClientInstanceProtocol,断开连接再创建新连接");
                    _protocol.Disconnect(false);
                    _protocol = null;
                }
                else
                {
                    LogHelper.logLisComm.Info("【LIS底层】创建新连接");
                }
                _protocol = new NetworkClientLISProtocol(autoResetEvent);
                StartAutoThread();
            }
            _protocol.Parameter = p_paramter;
            _protocol.Encoding = p_paramter.Encoding;
            return _protocol;
        }
        /// <summary>
        /// 根据协议，实例化服务端对象
        /// </summary>
        /// <param name="p_paramter"></param>
        /// <returns></returns>
        public ServiceLISProtocol GetServerInstanceProtocol(LISProtocolParameter p_paramter)
        {
            ServiceLISProtocol serverInstance = null;
            //网口
            if (p_paramter is NetworkParameter)
            {
                //if (_protocol != null)
                //{
                //    _protocol.Disconnect();
                //}
                serverInstance = new NetworkServiceLISProtocol();
                serverInstance.Parameter = p_paramter;
                serverInstance.Encoding = p_paramter.Encoding;
            }

            return serverInstance;
        }
        /// <summary>
        /// 发送线程
        /// </summary>
        Thread sendThread = null;

        /// <summary>
        /// 开始线程
        /// </summary>
        protected void StartAutoThread()
        {
            if (sendThread == null)
            {
                LogHelper.logLisComm.Info("【LIS底层】StartAutoThread");
                autoResetEvent.Reset();
                sendThread = new Thread(new ThreadStart(AutoThreadHandler));
                sendThread.IsBackground = true;
                sendThread.Start();
            }
            else
            {
                LogHelper.logLisComm.Info("【LIS底层】断网重连Thread存在");
            }
        }
        /// <summary>
        /// 自动重连线程
        /// </summary>
        /// <param name="obj"></param>
        private void AutoThreadHandler()
        {
            //延时6秒再进入判断
            Thread.Sleep(6000);
            try
            {
                while (true)
                {
                    //是否开启自动重连
                    if (_protocol.IsAutoConnect)
                    {
                        //自动重连间隔的时间
                        Thread.Sleep(_protocol.AutoConnectTime);
                        try
                        {
                            if (!_protocol.Connected)
                            {
                                LogHelper.logLisComm.Info("【LIS底层】断网尝试重连");
                                _protocol.Connect();
                            }
                            else
                            {
                                LogHelper.logLisComm.Info("【LIS底层】当前网络是连接状态，不需要断网重连");
                                autoResetEvent.Reset();
                                autoResetEvent.WaitOne();
                            }
                        }
                        catch (Exception e)
                        { }

                    }
                    else
                    {
                        LogHelper.logLisComm.Info("【LIS底层】没有开启断网重连");
                        autoResetEvent.Reset();
                        autoResetEvent.WaitOne();
                    }
                }


            }
            catch (ThreadAbortException abortException)
            {
                LogHelper.logLisComm.Info("【LIS底层】断网重连线程Abort");
            }
            catch (Exception e)
            {
                LogHelper.logLisComm.Info("【LIS底层】断网重连线程异常：" + e.ToString());
            }
        }


        /// <summary>
        /// 结束线程
        /// </summary>
        public void StopAutoConnectThread()
        {
            autoResetEvent.Reset();
            try
            {
                if (sendThread != null)
                {
                    LogHelper.logLisComm.Info("【LIS底层】StopAutoConnectThread");
                    sendThread.Abort();
                    sendThread = null;
                }
            }
            catch (ThreadAbortException ex)
            {
                //LogHelper.logSoftWare.Error("Stop Thread", ex);
            }
            catch (Exception e)
            {
                LogHelper.logLisComm.Info("【LIS底层】StopAutoConnectThread：" + e.ToString());
            }
            finally
            {
                sendThread = null;
            }
        }
    }
}
