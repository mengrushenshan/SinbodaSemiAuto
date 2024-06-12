using Sinboda.Framework.Common.Log;
using Sinboda.SemiAuto.Core.Helpers;
using Sinboda.SemiAuto.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Models.Common
{
    public class TcpDriver : ICommDriver
    {

        // 创建 TcpClient 实例
        private TcpClient tcpClient = new TcpClient();
        private string address;
        private int tcpPort;

        public TcpDriver(string add, int port)
        {
            address = add;
            tcpPort = port;
        }

        public void Dispose()
        {
            if (tcpClient.IsNull())
                return;
            tcpClient.Close();
            tcpClient.Dispose();
        }

        public bool IsConnected()
        {
            return tcpClient.Connected;
        }

        public byte[] Read()
        {
            try
            {
                //连接器断联
                if (tcpClient.Client.IsNull() || !tcpClient.Connected)
                {
                    LogHelper.logSoftWare.Info($"TcpDriver is not connected!");
                    return null;
                }

                //没有字节可读
                if (tcpClient.Available <= 0)
                {
                    //LogHelper.logSoftWare.Info($"tcpClient Available is null !");
                    return null;
                }

                //读取数据
                NetworkStream stream = tcpClient.GetStream();
                byte[] buffer = new byte[tcpClient.Available];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                //记录
                LogHelper.logSoftWare.Info($"TcpDriver Read: {buffer.ByteTo16Str()}");
                return buffer;

            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error($"TcpDriver Read error:{ex.Message}");
                return null;
            }
        }

        private static bool IsConnectionSuccessful = false;
        private static Exception socketexception;
        private static readonly ManualResetEvent TimeoutObject = new ManualResetEvent(false);

        public bool Connect()
        {
            try
            {
                TryConnect(address, tcpPort, 500);
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error($"TcpDriver Write error:{ex.Message}");
                return false;
            }

            return tcpClient.Connected;
        }

        private bool TryConnect(string address, int tcpPort, int timeoutMSec)
        {
            TimeoutObject.Reset();
            socketexception = null;

            tcpClient.BeginConnect(address, tcpPort,
                new AsyncCallback(CallBackMethod), tcpClient);

            if (TimeoutObject.WaitOne(timeoutMSec, false))
            {
                if (IsConnectionSuccessful)
                {
                    return true;
                }
                else
                {
                    throw socketexception;
                }
            }
            else
            {
                tcpClient.Close();
                throw new TimeoutException("TimeOut Exception");
            }
        }

        private static void CallBackMethod(IAsyncResult asyncresult)
        {
            try
            {
                IsConnectionSuccessful = false;
                TcpClient tcpclient = asyncresult.AsyncState as TcpClient;

                if (tcpclient.Client != null)
                {
                    tcpclient.EndConnect(asyncresult);
                    IsConnectionSuccessful = true;
                }
            }
            catch (Exception ex)
            {
                IsConnectionSuccessful = false;
                socketexception = ex;
            }
            finally
            {
                TimeoutObject.Set();
            }
        }

        public bool Write(byte[] bytes)
        {
            try
            {
                //连接器断联
                if (tcpClient.Client.IsNull() || !tcpClient.Connected)
                {
                    LogHelper.logSoftWare.Info($"TcpDriver is not connected!");
                    return false;
                }
                NetworkStream stream = tcpClient.GetStream();

                // 发送数据到服务器
                stream.Write(bytes, 0, bytes.Length);
                LogHelper.logSoftWare.Info($"TcpDriver Write: {bytes.ByteTo16Str()}");

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error($"TcpDriver Write error:{ex.Message}");
                return false;
            }
        }
    }
}
