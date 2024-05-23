using GalaSoft.MvvmLight.Messaging;
using Sinboda.Framework.Common.CommonFunc;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Communication.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sinboda.Framework.Communication.Utils.WriteLogs;

namespace Sinboda.Framework.Communication.DataPackages
{
    /// <summary>
    /// 串口数据拆包封包处理
    /// </summary>
    public class SerialPortPackageHeadHandle : ProtocolParameter, IPackageHandle
    {
        /// <summary>
        /// 拆包处理    
        /// </summary>
        /// <param name="recieveList"></param>
        /// <param name="dataPackage"></param>
        /// <returns></returns>
        public bool GetPackage(List<byte> recieveList, ref DataPackage dataPackage, CommunicateType types, string portName)
        {
            int typeSize = 9;
            //找到包头，移除无效数据
            if (recieveList.Count > 0)
            {
                byte[] data = GetOnePackageData(ref recieveList, portName);
                //剩余数据大于整包进行解析
                if (null != data && data.Length > typeSize)
                {
                    try
                    {
                        List<byte> list = new List<byte>();
                        list.Add(SerialPortHead);
                        //转换命令
                        byte[] dataCmdByte = new byte[4];
                        Array.Copy(data, 1, dataCmdByte, 0, 4);
                        byte[] dataCmdByteShort = TransferToShortValue(dataCmdByte).Reverse().ToArray();
                        list.AddRange(dataCmdByteShort.Reverse().ToArray());
                        ushort dataCmd = BitConverter.ToUInt16(dataCmdByteShort, 0);
                        //转换大小
                        byte[] dataLenByte = new byte[4];
                        Array.Copy(data, 5, dataLenByte, 0, 4);
                        byte[] dataLenByteShort = TransferToShortValue(dataLenByte).Reverse().ToArray();
                        list.AddRange(dataLenByteShort.Reverse().ToArray());
                        ushort dataLen = BitConverter.ToUInt16(dataLenByteShort, 0);
                        //包体部分
                        byte[] dataBodyByte = new byte[dataLen * 2];
                        Array.Copy(data, 9, dataBodyByte, 0, dataLen * 2);
                        byte[] dataBodyByteShort = TransferToShortValue(dataBodyByte);
                        list.AddRange(dataBodyByteShort);
                        //读取校验部分
                        byte[] dataCheckByte = new byte[4];
                        Array.Copy(data, dataLen * 2 + 9, dataCheckByte, 0, 4);
                        byte[] dataCheckByteShort = TransferToShortValue(dataCheckByte);
                        //进行校验
                        if (BitConverter.ToUInt16(CRC16(list.ToArray()), 0) != BitConverter.ToUInt16(dataCheckByteShort, 0))
                        {
                            LogHelper.logCommunication.Info("crc校验错误");
                            LogHelper.logCommunication.Info("<<<<<<<   Count:" + list.Count + "  Data " + BinaryUtilHelper.ByteToHex(list.ToArray()).ToString());
                            Messenger.Default.Send<string>("CRCCheckError", "CRCCheckError");
                            return false;
                        }
                        list.AddRange(dataCheckByteShort.Reverse().ToArray());
                        //读取包尾
                        //拷贝包头数据
                        byte[] dataBackByte = new byte[2];
                        Array.Copy(data, dataLen * 2 + 9 + 4, dataBackByte, 0, 2);
                        if (dataBackByte[0] != SerialPortEnd[0] || dataBackByte[1] != SerialPortEnd[1])
                        {
                            LogHelper.logCommunication.Info("包尾校验错误");
                            LogHelper.logCommunication.Info("<<<<<<<   Count:" + list.Count + "  Data " + BinaryUtilHelper.ByteToHex(list.ToArray()).ToString());
                            Messenger.Default.Send<string>("PackageEndCheckError", "PackageEndCheckError");
                            return false;
                        }
                        list.AddRange(dataBackByte);
                        dataPackage = PackageToDataPackage(dataCmd, dataBodyByteShort);
                        WriteLog(data, data.Length, communacationWay.RECIEVE, types, portName);
                        return true;
                    }
                    catch (Exception e)
                    {
                        LogHelper.logCommunication.Error("数据处理出错，错误信息为：" + e);
                        return false;
                    }
                }
                else return false;
            }
            else return false;
        }
        /// <summary>
        /// 获取一整包信息
        /// </summary>
        /// <param name="recieveList"></param>
        /// <param name="portName"></param>
        /// <returns></returns>
        public byte[] GetOnePackageData(ref List<byte> recieveList, string portName)
        {
            RemoveInvalidData(ref recieveList, portName);

            int nIndex = FindPackageEndPos(ref recieveList, portName);

            if (nIndex == int.MaxValue)
            {
                return null;
            }

            byte[] data = new byte[nIndex + 1];
            recieveList.CopyTo(0, data, 0, nIndex + 1);
            recieveList.RemoveRange(0, nIndex + 1);

            LinkedList<byte> finData = new LinkedList<byte>();
            for (int i = nIndex; i >= 0; i--)
            {
                finData.AddFirst(data[i]);
                if (data[i] == SerialPortHead)
                {
                    if (i != 0)
                    {
                        byte[] invalidArray = new byte[i];
                        Array.Copy(data, 0, invalidArray, 0, i);
                        LogHelper.logCommunication.Info(portName + "    包尾丢失数据：" + BinaryUtilHelper.ByteToHex(invalidArray).ToString());
                    }
                    break;
                }
            }

            return finData.ToArray();
        }

        /// <summary>
        /// 删除无效数据 0x02字段前的无效数据
        /// </summary>
        /// <param name="recieveList"></param>
        /// <param name="portName"></param>
        public void RemoveInvalidData(ref List<byte> recieveList, string portName)
        {
            List<byte> debugData = new List<byte>();
            for (int i = 0; i < recieveList.Count; i++)
            {
                if (recieveList[i] == SerialPortHead)
                {
                    break;
                }

                debugData.Add(recieveList[i]);
            }

            if (debugData.Count > 0)
            {
                LogHelper.logCommunication.Info(portName + "    非协议数据：" + BinaryUtilHelper.ByteToHex(debugData.ToArray()).ToString());
                recieveList.RemoveRange(0, debugData.Count);
            }
        }

        public int FindPackageEndPos(ref List<byte> recieveList, string portName)
        {
            int nIndexD = int.MaxValue;
            for (int i = 0; i < recieveList.Count; i++)
            {
                if (i >= 4000)
                {
                    LogHelper.logCommunication.Info("数据包长度异常：长度为" + recieveList.Count.ToString() + ",数据为:" + BinaryUtilHelper.ByteToHex(recieveList.ToArray()).ToString());
                    Messenger.Default.Send<string>("PackageLengthError", "PackageLengthError");
                    break;
                }

                if (recieveList[i] == 0x0d)
                {
                    nIndexD = i;
                }

                if (recieveList[i] == 0x0a && i >= 1 && nIndexD == i - 1)
                {
                    return i;
                }
            }

            return int.MaxValue;
        }

        /// <summary>
        /// 串口数据转换为平台包
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        DataPackage PackageToDataPackage(ushort cmd, byte[] data)
        {
            DataPackage result = new DataPackage();
            result.PackageInfo = new PackageInfo();
            result.PackageInfo.DataType = SerialPortHead;
            result.PackageInfo.BoardID = 0;
            result.PackageInfo.Module = 0;
            result.PackageInfo.Command = cmd;
            result.PackageInfo.PackNums = 1;
            result.PackageInfo.PackNo = 0;
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
            List<byte> list = new List<byte>();
            List<byte> checkList = new List<byte>();
            if (package.Data != null)
                data = new byte[1 + 4 + 4 + package.Data.Length * 2 + 4 + 2];
            else data = new byte[1 + 4 + 4 + 4 + 2];
            //包头
            list.Add(SerialPortHead);
            checkList.Add(SerialPortHead);
            //命令
            byte[] cmd = BitConverter.GetBytes((ushort)package.PackageInfo.Command);
            Array.Reverse(cmd);
            checkList.AddRange(cmd);
            byte[] cmdTransferd = TransferToLongValue(cmd);
            list.AddRange(cmdTransferd);
            if (package.Data != null)
            {
                //长度
                byte[] dataLen = BitConverter.GetBytes((ushort)package.Data.Length);
                Array.Reverse(dataLen);
                checkList.AddRange(dataLen);
                byte[] dataLenTransferd = TransferToLongValue(dataLen);
                list.AddRange(dataLenTransferd);

                //数据
                checkList.AddRange(package.Data);
                byte[] dataTransferd = TransferToLongValue(package.Data);
                list.AddRange(dataTransferd);
            }
            else
            {
                //长度
                byte[] dataLen = BitConverter.GetBytes((ushort)0);
                checkList.AddRange(dataLen);
                byte[] dataLenTransferd = TransferToLongValue(dataLen);
                list.AddRange(dataLenTransferd);
            }
            //校验位
            byte[] dataCheck = CRC16(checkList.ToArray());
            byte[] dataCheckTransferd = TransferToLongValue(dataCheck);
            list.AddRange(dataCheckTransferd);
            //包尾
            byte[] dataBack = SerialPortEnd;
            list.AddRange(dataBack);
            data = list.ToArray();
            return true;
        }


        /// <summary>
        /// 封包处理
        /// </summary>
        /// <param name="commnad"></param>
        /// <param name="orignalData"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SetPackageForSendEncyptData(ushort commnad, byte[] orignalData, byte[] key, ref byte[] data)
        {
            List<byte> dataTmp = new List<byte>();  // 临时数据，用于计算校验
            List<byte> dataSrc = new List<byte>();  // 加密前数据
            List<byte> dataDec = new List<byte>();  // 加密后数据

            // 包头
            dataTmp.Add(SerialPortHead);

            // 命令
            byte[] cmd = BitConverter.GetBytes(commnad);
            Array.Reverse(cmd);
            dataTmp.AddRange(cmd);
            dataSrc.AddRange(cmd);

            if (orignalData != null)
            {
                // 长度
                byte[] dataLen = BitConverter.GetBytes((ushort)orignalData.Length);
                Array.Reverse(dataLen);
                dataTmp.AddRange(dataLen);
                dataSrc.AddRange(dataLen);

                // 数据
                dataTmp.AddRange(orignalData);
                dataSrc.AddRange(orignalData);
            }
            else
            {
                //长度
                byte[] dataLen = BitConverter.GetBytes((ushort)0);
                dataTmp.AddRange(dataLen);
                dataSrc.AddRange(dataLen);
            }

            // 校验位
            byte[] dataCheck = CRC16(dataTmp.ToArray());
            dataSrc.AddRange(dataCheck);

            // DES加密
            byte[] bytes = Helper.EncryptDES(dataSrc.ToArray(), key);

            // 准备输出
            dataDec.Add(SerialPortHead);
            dataDec.AddRange(TransferToLongValue(bytes));
            dataDec.AddRange(SerialPortEnd);

            // 赋值返回
            data = dataDec.ToArray();

            return true;
        }
        /// <summary>
        /// 拆包处理    
        /// </summary>
        /// <param name="recieveList"></param>
        /// <param name="dataPackage"></param>
        /// <returns></returns>
        public bool GetPackageForReceiveEncyptData(List<byte> recieveList, byte[] key, ref DataPackage dataPackage, CommunicateType types, string portName)
        {
            List<byte> lsBody = new List<byte>();
            List<byte> lsALL = new List<byte>();
            int dataCount = 0;
            for (int i = 0; i < recieveList.Count; i++)
            {
                if (recieveList[i] == SerialPortHead)
                {
                    continue;
                }
                else if (recieveList[i + 1] == 0x0a && recieveList[i] == 0x0d)
                {
                    dataCount = i + 1;
                    continue;
                }
                else if (recieveList[i] == 0x0a && recieveList[i - 1] == 0x0d)
                {
                    dataCount = i + 1;
                    break;
                }
                else
                {
                    lsBody.Add(recieveList[i]);
                }
            }
            byte[] datast = TransferToShortValue(lsBody.ToArray());
            byte[] dataen = Helper.DecryptDES(datast, key);
            lsALL.Add(SerialPortHead);
            lsALL.AddRange(dataen);
            lsALL.AddRange(SerialPortEnd);

            int typeSize = 9;
            List<byte> debugData = new List<byte>();
            //找到包头，移除无效数据
            if (recieveList.Count > 0)
            {
                for (int i = 0; i < recieveList.Count; i++)
                {
                    if (recieveList[i] == SerialPortHead)
                        break;
                    else
                        debugData.Add(recieveList[i]);
                }
                if (debugData.Count > 0)
                {
                    LogHelper.logCommunication.Debug("非协议数据：" + BinaryUtilHelper.ByteToASCII(debugData.ToArray()));
                    recieveList.RemoveRange(0, debugData.Count);
                    dataPackage = null; //PackageToDataPackage(0, debugData.ToArray());
                    return true;
                }
                //剩余数据大于包头进行解析
                if (lsALL.Count > typeSize)
                {

                    //剩余数据大于整包进行解析
                    if (lsALL.Contains(0x0d) && lsALL.Contains(0x0a))
                    {
                        byte[] data = new byte[10000];
                        //将数据移入到数组中
                        for (int i = 0; i < lsALL.Count; i++)
                        {
                            if (i == 4000)
                            {
                                LogHelper.logCommunication.Info("数据包长度异常：长度为" + recieveList.Count.ToString() + ",数据为:" + BinaryUtilHelper.ByteToHex(recieveList.ToArray()).ToString());
                                Messenger.Default.Send<string>("PackageLengthError", BinaryUtilHelper.ByteToHex(recieveList.ToArray()).ToString());
                                break;
                            }
                            data[i] = lsALL[i];
                            if (lsALL[i] == 0x0a && lsALL[i - 1] == 0x0d)
                            {
                                break;
                            }
                        }
                        //移除数据
                        recieveList.RemoveRange(0, dataCount);

                        try
                        {
                            List<byte> list = new List<byte>();
                            list.Add(SerialPortHead);
                            //转换命令
                            byte[] dataCmdByte = new byte[2];
                            Array.Copy(data, 1, dataCmdByte, 0, 2);
                            byte[] dataCmdByteShort = dataCmdByte.Reverse().ToArray();// TransferToShortValue(dataCmdByte).Reverse().ToArray();
                            list.AddRange(dataCmdByte.ToArray());
                            ushort dataCmd = BitConverter.ToUInt16(dataCmdByteShort, 0);
                            //转换大小
                            byte[] dataLenByte = new byte[2];
                            Array.Copy(data, 3, dataLenByte, 0, 2);
                            byte[] dataLenByteShort = dataLenByte.Reverse().ToArray();// TransferToShortValue(dataLenByte).Reverse().ToArray();
                            list.AddRange(dataLenByte.ToArray());
                            ushort dataLen = BitConverter.ToUInt16(dataLenByteShort, 0);
                            //包体部分
                            byte[] dataBodyByte = new byte[dataLen];
                            Array.Copy(data, 5, dataBodyByte, 0, dataLen);
                            byte[] dataBodyByteShort = dataBodyByte;// TransferToShortValue(dataBodyByte);
                            list.AddRange(dataBodyByteShort);
                            //读取校验部分
                            byte[] dataCheckByte = new byte[2];
                            Array.Copy(data, dataLen + 5, dataCheckByte, 0, 2);
                            byte[] dataCheckByteShort = dataCheckByte.Reverse().ToArray();// TransferToShortValue(dataCheckByte);
                            //进行校验
                            if (BitConverter.ToUInt16(CRC16(list.ToArray()), 0) != BitConverter.ToUInt16(dataCheckByte, 0))
                            {
                                LogHelper.logCommunication.Info("crc校验错误");
                                LogHelper.logCommunication.Info("<<<<<<<   Count:" + list.Count + "  Data " + BinaryUtilHelper.ByteToHex(list.ToArray()).ToString());
                                Messenger.Default.Send<string>("CRCCheckError", BinaryUtilHelper.ByteToHex(list.ToArray()).ToString());
                                return false;
                            }
                            list.AddRange(dataCheckByte.ToArray());
                            //读取包尾
                            //拷贝包头数据
                            byte[] dataBackByte = new byte[2];
                            Array.Copy(data, dataLen + 5 + 2, dataBackByte, 0, 2);
                            if (dataBackByte[0] != SerialPortEnd[0] || dataBackByte[1] != SerialPortEnd[1])
                            {
                                LogHelper.logCommunication.Info("包尾校验错误");
                                LogHelper.logCommunication.Info("<<<<<<<   Count:" + list.Count + "  Data " + BinaryUtilHelper.ByteToHex(list.ToArray()).ToString());
                                Messenger.Default.Send<string>("PackageEndCheckError", BinaryUtilHelper.ByteToHex(list.ToArray()).ToString());
                                return false;
                            }
                            list.AddRange(dataBackByte);
                            dataPackage = PackageToDataPackage(dataCmd, dataBodyByteShort);
                            WriteLogForReceiveEncryptData(data, list.Count, communacationWay.RECIEVE, types, portName);
                            return true;
                        }
                        catch (Exception e)
                        {
                            LogHelper.logCommunication.Error("数据处理出错，错误信息为：" + e);
                            return false;
                        }
                    }
                    else return false;
                }
                else return false;
            }
            else return false;
        }

        /// <summary>
        /// 字符长数据转换成短数据
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] TransferToShortValue(byte[] bytes)
        {
            byte[] bytesTmp = new byte[bytes.Length / 2];
            try
            {
                for (int i = 0; i < bytes.Length / 2; i++)
                    bytesTmp[i] = Convert.ToByte("0x" + Convert.ToChar(bytes[i * 2]).ToString() + Convert.ToChar(bytes[i * 2 + 1]).ToString(), 16);
            }
            catch (Exception e)
            {
                LogHelper.logCommunication.Error("TransferToShortValue() 数据处理出错，错误信息为：" + e);
                LogHelper.logCommunication.Error("TransferToShortValue() 原始转化数据:" + BinaryUtilHelper.ByteToHex(bytes).ToString());
            }
            return bytesTmp;
        }
        /// <summary>
        /// 字符短数据转换成长数据
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        static byte[] TransferToLongValue(byte[] bytes)
        {
            byte[] bytesTmp = new byte[bytes.Length * 2];
            string str = string.Empty;
            for (int i = 0; i < bytes.Length; i++)
            {
                str = bytes[i].ToString("X2");
                bytesTmp[2 * i] = (byte)str[0];
                bytesTmp[2 * i + 1] = (byte)str[1];
            }
            return bytesTmp;
        }
        /// <summary>
        /// crc校验
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static byte[] CRC16(byte[] data)
        {
            byte i = 0;
            ushort crc = 0;
            for (int j = 0; j < data.Length; j++)
            {
                for (i = 0x80; i != 0; i >>= 1)
                {
                    if ((crc & 0x8000) != 0)
                    {
                        crc <<= 1;
                        crc ^= 0x1021;
                    }
                    else
                        crc <<= 1;

                    if ((data[j] & i) != 0)
                        crc ^= 0x1021;
                }
            }
            return BitConverter.GetBytes(crc).Reverse().ToArray();
        }
    }
}
