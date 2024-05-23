using Sinboda.Framework.Common.CommonFunc;
using Sinboda.Framework.Common.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Sinboda.Framework.Communication.Utils.WriteLogs;

namespace Sinboda.Framework.Communication.DataPackages
{
    /// <summary>
    /// 网口数据拆包封包处理
    /// </summary>
    public class NetworkPackageHeadHandle : ProtocolParameter, IPackageHandle
    {
        /// <summary>
        /// 拆包处理    
        /// </summary>
        /// <param name="recieveList"></param>
        /// <param name="dataPackage"></param>
        /// <returns></returns>
        public bool GetPackage(List<byte> recieveList, ref DataPackage dataPackage, CommunicateType types, string portName)
        {
            try
            {
                NetworkPackageHead headPak = new NetworkPackageHead();
                Type type = typeof(NetworkPackageHead);
                int typeSize = Marshal.SizeOf(type);
                List<byte> errorData = new List<byte>();
                //找到包头，移除无效数据
                if (recieveList.Count > 0)
                {
                    for (int i = 0; i < recieveList.Count; i++)
                    {
                        if (recieveList[i] == NetworkHead)
                        {
                            LogHelper.logCommunication.Debug("finded network head");
                            // 找到包长度数据
                            byte[] nBodyLenByteTmp = new byte[2] { recieveList[i + 12], recieveList[i + 11] };
                            int dataLength = BitConverter.ToUInt16(nBodyLenByteTmp, 0);
                            if (dataLength <= 1024 && (recieveList[i + 13] > recieveList[i + 14] || (recieveList[i + 13] == recieveList[i + 14] + 1)))
                            {
                                LogHelper.logCommunication.Debug("completed netword head");
                                break;
                            }
                            else
                                errorData.Add(recieveList[i]);
                        }
                        else
                            errorData.Add(recieveList[i]);
                    }
                    if (errorData.Count > 0)
                    {
                        LogHelper.logCommunication.Info("非协议数据：" + BinaryUtilHelper.ByteToHex(errorData.ToArray()).ToString());
                        recieveList.RemoveRange(0, errorData.Count);
                    }
                    //剩余数据大于包头进行解析
                    if (recieveList.Count >= typeSize)
                    {
                        LogHelper.logCommunication.Debug("list length more than struct size");
                        //找到包长度数据
                        byte[] nBodyLenByteTmp = new byte[2] { recieveList[12], recieveList[11] };
                        int dataLength = BitConverter.ToUInt16(nBodyLenByteTmp, 0);
                        //剩余数据大于整包进行解析
                        if (recieveList.Count >= typeSize + dataLength)
                        {
                            LogHelper.logCommunication.Debug($"list length can get data package,datalength is {dataLength}");
                            byte[] data = new byte[typeSize + dataLength];
                            //将数据移入到数组中
                            for (int i = 0; i < data.Length; i++)
                            {
                                data[i] = recieveList[i];
                            }
                            //移除数据
                            recieveList.RemoveRange(0, data.Length);

                            //LogHelper.logCommunication.Debug($"--------analysis begin---------------");
                            //WriteLog(recieveList.ToArray(), recieveList.Count, communacationWay.RECIEVE, CommunicateType.Network);
                            //LogHelper.logCommunication.Debug($"---------analysis end--------------");

                            //开始解析
                            List<byte> list = new List<byte>();
                            //协议头
                            byte nDataTypeByte = data[0];
                            headPak.nDataType = nDataTypeByte;
                            list.Add(nDataTypeByte);
                            //当前板号
                            byte[] nCurrentSourceBoardIDByte = new byte[2] { data[2], data[1] };
                            list.AddRange(nCurrentSourceBoardIDByte.Reverse().ToArray());
                            headPak.nCurrentSourceBoardID = BitConverter.ToUInt16(nCurrentSourceBoardIDByte, 0);
                            //目标板号
                            byte[] nCurrentDestinitionBoardIDByte = new byte[2] { data[4], data[3] };
                            list.AddRange(nCurrentDestinitionBoardIDByte.Reverse().ToArray());
                            headPak.nCurrentDestinitionBoardID = BitConverter.ToUInt16(nCurrentDestinitionBoardIDByte, 0);
                            //原始板号
                            byte[] nOriginalSourceBoardIDByte = new byte[2] { data[6], data[5] };
                            list.AddRange(nOriginalSourceBoardIDByte.Reverse().ToArray());
                            headPak.nOriginalSourceBoardID = BitConverter.ToUInt16(nOriginalSourceBoardIDByte, 0);
                            //原始目标板号
                            byte[] nOriginalDestinitionBoardIDByte = new byte[2] { data[8], data[7] };
                            list.AddRange(nOriginalDestinitionBoardIDByte.Reverse().ToArray());
                            headPak.nOriginalDestinitionBoardID = BitConverter.ToUInt16(nOriginalDestinitionBoardIDByte, 0);
                            //命令
                            byte[] nCommandByte = new byte[2] { data[10], data[9] };
                            list.AddRange(nCommandByte.Reverse().ToArray());
                            headPak.nCommand = BitConverter.ToUInt16(nCommandByte, 0);

                            LogHelper.logCommunication.Debug($"datapackage command is {headPak.nCommand}");

                            //包体长度
                            byte[] nBodyLenByte = new byte[2] { data[12], data[11] };
                            list.AddRange(nBodyLenByte.Reverse().ToArray());
                            headPak.nBodyLen = BitConverter.ToUInt16(nBodyLenByte, 0);
                            //包数量
                            byte nPackNumsByte = data[13];
                            headPak.nPackNums = nPackNumsByte;
                            list.Add(nPackNumsByte);
                            //包序号
                            byte nPackNoByte = data[14];
                            headPak.nPackNo = nPackNoByte;
                            list.Add(nPackNoByte);
                            //包体
                            byte[] dataBody = null;
                            if (headPak.nBodyLen > 0)
                            {
                                dataBody = new byte[headPak.nBodyLen];
                                Array.Copy(data, 15, dataBody, 0, headPak.nBodyLen);
                                list.AddRange(dataBody);
                            }
                            WriteLog(list.ToArray(), list.Count, communacationWay.RECIEVE, types, portName);
                            dataPackage = PackageToDataPackage(headPak, dataBody);
                            return true;
                        }
                        else return false;
                    }
                    else return false;
                }
                else return false;
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();

                foreach (byte b in recieveList)
                {
                    sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
                    sb.Append(" ");
                }
                LogHelper.logCommunication.Debug($"拆包处理：{sb.ToString()} , {ex.Message}");

                return false;
            }
        }
        /// <summary>
        /// 拆包处理    
        /// </summary>
        /// <param name="recieveList"></param>
        /// <param name="dataPackage"></param>
        /// <returns></returns>
        public bool GetPackageForReceiveEncyptData(List<byte> recieveList, byte[] key, ref DataPackage dataPackage, CommunicateType types, string portName)
        {
            try
            {
                NetworkPackageHead headPak = new NetworkPackageHead();
                Type type = typeof(NetworkPackageHead);
                int typeSize = Marshal.SizeOf(type);
                List<byte> errorData = new List<byte>();
                //找到包头，移除无效数据
                if (recieveList.Count > 0)
                {
                    for (int i = 0; i < recieveList.Count; i++)
                    {
                        if (recieveList[i] == NetworkHead)
                        {
                            // 找到包长度数据
                            byte[] nBodyLenByteTmp = new byte[2] { recieveList[i + 12], recieveList[i + 11] };
                            int dataLength = BitConverter.ToUInt16(nBodyLenByteTmp, 0);
                            if (dataLength <= 1024 && (recieveList[i + 13] > recieveList[i + 14] || (recieveList[i + 13] == recieveList[i + 14] + 1)))
                                break;
                            else
                                errorData.Add(recieveList[i]);
                        }
                        else
                            errorData.Add(recieveList[i]);
                    }
                    if (errorData.Count > 0)
                    {
                        LogHelper.logCommunication.Info("非协议数据：" + BinaryUtilHelper.ByteToHex(errorData.ToArray()).ToString());
                        recieveList.RemoveRange(0, errorData.Count);
                    }
                    //剩余数据大于包头进行解析
                    if (recieveList.Count > typeSize)
                    {
                        //找到包长度数据
                        byte[] nBodyLenByteTmp = new byte[2] { recieveList[12], recieveList[11] };
                        int dataLength = BitConverter.ToUInt16(nBodyLenByteTmp, 0);
                        //剩余数据大于整包进行解析
                        if (recieveList.Count >= typeSize + dataLength)
                        {
                            byte[] data = new byte[typeSize + dataLength];
                            //将数据移入到数组中
                            for (int i = 0; i < data.Length; i++)
                            {
                                data[i] = recieveList[i];
                            }
                            //移除数据
                            recieveList.RemoveRange(0, data.Length);
                            //开始解析
                            List<byte> list = new List<byte>();
                            //协议头
                            byte nDataTypeByte = data[0];
                            headPak.nDataType = nDataTypeByte;
                            list.Add(nDataTypeByte);
                            //当前板号
                            byte[] nCurrentSourceBoardIDByte = new byte[2] { data[2], data[1] };
                            list.AddRange(nCurrentSourceBoardIDByte.Reverse().ToArray());
                            headPak.nCurrentSourceBoardID = BitConverter.ToUInt16(nCurrentSourceBoardIDByte, 0);
                            //目标板号
                            byte[] nCurrentDestinitionBoardIDByte = new byte[2] { data[4], data[3] };
                            list.AddRange(nCurrentDestinitionBoardIDByte.Reverse().ToArray());
                            headPak.nCurrentDestinitionBoardID = BitConverter.ToUInt16(nCurrentDestinitionBoardIDByte, 0);
                            //原始板号
                            byte[] nOriginalSourceBoardIDByte = new byte[2] { data[6], data[5] };
                            list.AddRange(nOriginalSourceBoardIDByte.Reverse().ToArray());
                            headPak.nOriginalSourceBoardID = BitConverter.ToUInt16(nOriginalSourceBoardIDByte, 0);
                            //原始目标板号
                            byte[] nOriginalDestinitionBoardIDByte = new byte[2] { data[8], data[7] };
                            list.AddRange(nOriginalDestinitionBoardIDByte.Reverse().ToArray());
                            headPak.nOriginalDestinitionBoardID = BitConverter.ToUInt16(nOriginalDestinitionBoardIDByte, 0);
                            //命令
                            byte[] nCommandByte = new byte[2] { data[10], data[9] };
                            list.AddRange(nCommandByte.Reverse().ToArray());
                            headPak.nCommand = BitConverter.ToUInt16(nCommandByte, 0);
                            //包体长度
                            byte[] nBodyLenByte = new byte[2] { data[12], data[11] };
                            list.AddRange(nBodyLenByte.Reverse().ToArray());
                            headPak.nBodyLen = BitConverter.ToUInt16(nBodyLenByte, 0);
                            //包数量
                            byte nPackNumsByte = data[13];
                            headPak.nPackNums = nPackNumsByte;
                            list.Add(nPackNumsByte);
                            //包序号
                            byte nPackNoByte = data[14];
                            headPak.nPackNo = nPackNoByte;
                            list.Add(nPackNoByte);
                            //包体
                            byte[] dataBody = new byte[headPak.nBodyLen];
                            Array.Copy(data, 15, dataBody, 0, headPak.nBodyLen);
                            list.AddRange(dataBody);
                            WriteLog(list.ToArray(), list.Count, communacationWay.RECIEVE, types);
                            dataPackage = PackageToDataPackage(headPak, dataBody);
                            return true;
                        }
                        else return false;
                    }
                    else return false;
                }
                else return false;
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();

                foreach (byte b in recieveList)
                {
                    sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
                    sb.Append(" ");
                }
                LogHelper.logCommunication.Debug($"拆包处理：{sb.ToString()} , {ex.Message}");

                return false;
            }
        }
        /// <summary>
        /// 网络数据转换为平台包
        /// </summary>
        /// <param name="pak"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        DataPackage PackageToDataPackage(NetworkPackageHead pak, byte[] data)
        {
            DataPackage result = new DataPackage();
            result.PackageInfo = new PackageInfo();
            result.PackageInfo.DataType = pak.nDataType;
            result.PackageInfo.BoardID = pak.nCurrentSourceBoardID;
            result.PackageInfo.Module = pak.nOriginalSourceBoardID;
            result.PackageInfo.Command = pak.nCommand;
            result.PackageInfo.PackNums = pak.nPackNums;
            result.PackageInfo.PackNo = pak.nPackNo;
            result.Data = data;
            return result;
        }
        /// <summary>
        /// 封包处理
        /// </summary>
        /// <param name="package"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SetPackage(DataPackage package, ref byte[] data)
        {
            Type type = typeof(NetworkPackageHead);
            int typeSize = Marshal.SizeOf(type);
            List<byte> list = new List<byte>();
            //添加包头协议
            list.Add(NetworkHead);
            //添加当前原板号
            list.AddRange(BitConverter.GetBytes((ushort)NetworkOriginalID).Reverse().ToArray());
            //添加当前目标板号
            list.AddRange(BitConverter.GetBytes((ushort)package.PackageInfo.BoardID).Reverse().ToArray());
            //添加原始板号
            list.AddRange(BitConverter.GetBytes((ushort)NetworkOriginalID).Reverse().ToArray());
            //添加原始目标板号
            list.AddRange(BitConverter.GetBytes((ushort)package.PackageInfo.Module).Reverse().ToArray());
            //添加命令
            list.AddRange(BitConverter.GetBytes((ushort)package.PackageInfo.Command).Reverse().ToArray());
            //包体数据不为空
            if (package.Data != null)
                list.AddRange(BitConverter.GetBytes((ushort)package.Data.Length).Reverse().ToArray());
            else
                list.AddRange(BitConverter.GetBytes((ushort)0).Reverse().ToArray());
            //添加包数量
            list.Add((byte)package.PackageInfo.PackNums);
            //添加包序号
            list.Add((byte)package.PackageInfo.PackNo);
            //包体数据不为空
            if (package.Data != null)
                list.AddRange(package.Data);
            data = list.ToArray();
            return true;
        }
        /// <summary>
        /// 封包处理
        /// </summary>
        /// <param name="package"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SetPackageForSendEncyptData(ushort commnad, byte[] orignalData, byte[] key, ref byte[] data)
        {
            //Type type = typeof(NetworkPackageHead);
            //int typeSize = Marshal.SizeOf(type);
            //List<byte> list = new List<byte>();
            ////添加包头协议
            //list.Add(NetworkHead);
            ////添加当前原板号
            //list.AddRange(BitConverter.GetBytes((ushort)NetworkOriginalID).Reverse().ToArray());
            ////添加当前目标板号
            //list.AddRange(BitConverter.GetBytes((ushort)package.PackageInfo.BoardID).Reverse().ToArray());
            ////添加原始板号
            //list.AddRange(BitConverter.GetBytes((ushort)NetworkOriginalID).Reverse().ToArray());
            ////添加原始目标板号
            //list.AddRange(BitConverter.GetBytes((ushort)package.PackageInfo.Module).Reverse().ToArray());
            ////添加命令
            //list.AddRange(BitConverter.GetBytes((ushort)package.PackageInfo.Command).Reverse().ToArray());
            ////包体数据不为空
            //if (package.Data != null)
            //    list.AddRange(BitConverter.GetBytes((ushort)package.Data.Length).Reverse().ToArray());
            //else
            //    list.AddRange(BitConverter.GetBytes((ushort)0).Reverse().ToArray());
            ////添加包数量
            //list.Add((byte)package.PackageInfo.PackNums);
            ////添加包序号
            //list.Add((byte)package.PackageInfo.PackNo);
            ////包体数据不为空
            //if (package.Data != null)
            //    list.AddRange(package.Data);
            //data = list.ToArray();
            return true;
        }
    }
}
