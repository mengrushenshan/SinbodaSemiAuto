using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Communication.DataPackages
{
    /// <summary>
    /// 包处理接口
    /// </summary>
    public interface IPackageHandle
    {
        /// <summary>
        /// 读取缓存区数据转换为 DataPackage
        /// </summary>
        /// <param name="recieveList"></param>
        /// <param name="pakage"></param>
        /// <returns></returns>
        bool GetPackage(List<byte> recieveList, ref DataPackage pakage, CommunicateType types, string portName = "");
        /// <summary>
        /// 读取缓存区数据转换为 DataPackage
        /// </summary>
        /// <param name="recieveList"></param>
        /// <param name="pakage"></param>
        /// <returns></returns>
        bool GetPackageForReceiveEncyptData(List<byte> recieveList, byte[] key, ref DataPackage pakage, CommunicateType types, string portName = "");
        /// <summary>
        /// DataPackage 转化为 byte[] 
        /// </summary>
        /// <param name="pakage"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        bool SetPackage(DataPackage pakage, ref byte[] data);
        /// <summary>
        /// DataPackage 转化为 byte[] 
        /// </summary>
        /// <param name="pakage"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        bool SetPackageForSendEncyptData(ushort commnad, byte[] orignalData, byte[] key, ref byte[] sendData);
    }
}
