using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.BusinessModels
{
    /// <summary>
    /// LIS通信设置实体
    /// </summary>
    public class LISCommunicationInterfaceModel
    {
        /// <summary>
        /// LIS通信是否启用
        /// </summary>
        public bool LISEnabled { get; set; }
        /// <summary>
        /// 仪器ID
        /// </summary>
        public string MachineID { get; set; }
        /// <summary>
        /// LISID
        /// </summary>
        public string LISID { get; set; }

        /// <summary>
        /// 是否启用网口
        /// </summary>
        public bool IsNetWork { get; set; }
        /// <summary>
        /// 网口IP
        /// </summary>
        public string NetworkIP { get; set; }
        /// <summary>
        /// 网口端口号
        /// </summary>
        public int NetworkPort { get; set; }
        /// <summary>
        /// 串口号
        /// </summary>
        public string SerialPort { get; set; }
        /// <summary>
        /// 波特率
        /// </summary>
        public string BaudRate { get; set; }
        /// <summary>
        /// 数据位
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// 停止位
        /// </summary>
        public string StopType { get; set; }
        /// <summary>
        /// 校验位
        /// </summary>
        public string CheckType { get; set; }
    }
}
