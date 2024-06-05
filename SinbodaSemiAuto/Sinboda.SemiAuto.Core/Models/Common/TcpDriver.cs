using Sinboda.Framework.Common.Log;
using Sinboda.SemiAuto.Core.Helpers;
using Sinboda.SemiAuto.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Models.Common
{
    public class TcpDriver : ICommDriver
    {

        // 创建 TcpClient 实例
        private TcpClient tcpClient;
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
                if (!tcpClient.Connected)
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

        public bool Connect()
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(address, tcpPort);
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error($"TcpDriver Write error:{ex.Message}");
                return false;
            }

            return tcpClient.Connected;
        }

        public bool Write(byte[] bytes)
        {
            try
            {
                //连接器断联
                if (!tcpClient.Connected)
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
