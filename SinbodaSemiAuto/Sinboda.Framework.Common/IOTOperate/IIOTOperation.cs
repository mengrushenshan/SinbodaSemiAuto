using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Common.IOTOperate
{
    /// <summary>
    /// IOT交互的接口
    /// </summary>
    public interface IIOTOperation
    {
        /// <summary>
        /// 获取三元组信息
        /// </summary>
        /// <returns></returns>
        DeviceOptionsModel GetDeviceOptions();
        /// <summary>
        /// 需要执行的SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        DataTable ExecuteSql(string sql);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsBusy();
    }
}
