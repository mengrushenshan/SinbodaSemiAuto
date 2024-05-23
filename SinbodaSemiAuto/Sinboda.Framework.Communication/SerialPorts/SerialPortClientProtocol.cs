using Sinboda.Framework.Common.Log;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Communication.SerialPorts
{
    /// <summary>
    /// 串口客户端模式
    /// </summary>
    public class SerialPortClientProtocol : ClientDeviceProtocol
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SerialPortClientProtocol() : base()
        {

        }
        /// <summary>
        /// 串口通讯
        /// </summary>
        protected SerialPort _serialPort = new SerialPort();
        /// <summary>
        /// 通讯参数
        /// </summary>
        /// <returns></returns>
        protected override ProtocolParameter CreateParameterInstance()
        {
            return (new SerialPortParameter());
        }
        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public override bool Connect()
        {
            if (Connected)
            {
                this.Disconnect();
            }
            _serialPort.ReadBufferSize = (Parameter as SerialPortParameter).ReceiveBufferSize;
            _serialPort.WriteBufferSize = (Parameter as SerialPortParameter).SendBufferSize;
            _serialPort.PortName = (Parameter as SerialPortParameter).PortName;
            _serialPort.BaudRate = (Parameter as SerialPortParameter).BaudRate;
            _serialPort.DataBits = (Parameter as SerialPortParameter).DataBits;
            _serialPort.Parity = (Parameter as SerialPortParameter).Parity;
            try
            {
                _serialPort.StopBits = (Parameter as SerialPortParameter).StopBits;
                _serialPort.Open();

                StartThreads();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.logCommunication.Error("SerialPort Connect error", ex);
                return false;
            }
        }
        /// <summary>
        /// 断开
        /// </summary>
        public override void Disconnect()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
                StopThreads();
            }

        }
        /// <summary>
        /// 是否连接
        /// </summary>
        /// <returns></returns>
        protected override bool GetConnected()
        {
            return _serialPort.IsOpen;
        }
        /// <summary>
        /// 通讯类别
        /// </summary>
        /// <returns></returns>
        protected override CommunicateType GetCommunicateType()
        {
            return CommunicateType.SerialPort;
        }
        /// <summary>
        /// 发数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected override bool WriteBuffer(byte[] buffer, int offset, int count)
        {
            try
            {
                _serialPort.Write(buffer, offset, count);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.logCommunication.Error("SerialPort WriteBuffer error", ex);
                return false;
            }
        }
        /// <summary>
        /// 收数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected override int ReadBuffer(byte[] buffer, int offset, int count)
        {
            try
            {
                return _serialPort.Read(buffer, offset, count);
            }
            catch (Exception ex)
            {
                LogHelper.logCommunication.Error("SerialPort ReadBuffer error", ex);
                return -1;
            }
        }
    }
}
