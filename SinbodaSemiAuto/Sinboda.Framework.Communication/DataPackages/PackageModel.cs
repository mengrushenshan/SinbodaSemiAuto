using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Communication.DataPackages
{
    /// <summary>
    /// 网口协议包头
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct NetworkPackageHead
    {
        /// <summary>
        /// 协议版本
        /// </summary>
        public byte nDataType;
        /// <summary>
        /// 当前板号或模块号
        /// </summary>
        public ushort nCurrentSourceBoardID;
        /// <summary>
        /// 当前目标板号或模块号
        /// </summary>
        public ushort nCurrentDestinitionBoardID;
        /// <summary>
        /// 原始版号或模块号
        /// </summary>
        public ushort nOriginalSourceBoardID;
        /// <summary>
        /// 原始目标板号或模块号
        /// </summary>
        public ushort nOriginalDestinitionBoardID;
        /// <summary>
        /// 命令
        /// </summary>
        public ushort nCommand;
        /// <summary>
        /// 包体长度
        /// </summary>
        public ushort nBodyLen;
        /// <summary>
        /// 发送总包数
        /// </summary>
        public byte nPackNums;
        /// <summary>
        /// 发送包序号
        /// </summary>
        public byte nPackNo;
    }
    /// <summary>
    /// 串口协议包头
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct SerialPortPackageHead
    {
        /// <summary>
        /// 协议起始
        /// </summary>
        public byte nPacketStart;
        /// <summary>
        /// 命令
        /// </summary>
        public ushort nCommand;
        /// <summary>
        /// 长度
        /// </summary>
        public ushort nDataLen;
    }
    /// <summary>
    /// 包头信息
    /// </summary>
    public class PackageInfo
    {
        /// <summary>
        /// 协议版本
        /// </summary>
        public int DataType { get; set; }
        /// <summary>
        /// 版号
        /// </summary>
        public int BoardID { get; set; }
        /// <summary>
        /// 模块号
        /// </summary>
        public int Module { get; set; }
        /// <summary>
        /// 命令
        /// </summary>
        public int Command { get; set; }
        /// <summary>
        /// 发送总包数
        /// </summary>
        public int PackNums { get; internal set; }
        /// <summary>
        /// 发送包序号
        /// </summary>
        public int PackNo { get; internal set; }
        /// <summary>
        /// ID信息
        /// </summary>
        public string ID { get; set; }
    }
    /// <summary>
    /// 原始数据打包信息
    /// </summary>
    public class DataPackage
    {
        /// <summary>
        /// 包信息
        /// </summary>
        public PackageInfo PackageInfo { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public byte[] Data { get; set; }
    }
}
