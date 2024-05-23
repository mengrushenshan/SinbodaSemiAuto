using Sinboda.Framework.Common.CommonFunc;
using Sinboda.Framework.Common.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sinboda.Framework.Common.IOTOperate
{
    public delegate DeviceOptionsModel DeviceEvent(IotHeader<DeviceOptionsModel> iot);

    public delegate object ExecuteEvent(IotHeader<object> iot, string type, string value);

    public delegate bool StatusEvent(IotHeader<IsBusyStruct> iot);

    public class IOTOperateManager
    {
        public static readonly string targetTitle = "Diruiyun.Iot.Service.Client";

        /// <summary>
        /// 注册信号量
        /// </summary>
        private static AutoResetEvent regAutoResetEvent = new AutoResetEvent(false);
        private static string code, regCode;


        public static event ExecuteEvent IOTExecuteEvent;
        /// <summary>
        /// 按照type执行，返回json串
        /// </summary>
        /// <param name="args">type</param>
        /// <param name="title">windowName</param>
        public static void OnExecuteEvent(string argsType, string argsValue, string title)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                string json = string.Empty;
                try
                {
                    if (IOTExecuteEvent == null || string.IsNullOrEmpty(argsType))
                        return;

                    IotHeader<object> iot = new IotHeader<object>();
                    iot.method = "execute";
                    iot.code = "200";
                    iot.data = IOTExecuteEvent(iot, argsType, argsValue);
                    json = JsonConvert.SerializeObject(iot);
                }
                catch (Exception ex)
                {
                    IotHeader<string> error = new IotHeader<string>();
                    error.data = ex.Message;
                    error.method = "execute";
                    error.code = "400";
                    json = JsonConvert.SerializeObject(error);
                    LogHelper.logSoftWare.Debug($"[远程 OnExecuteEvent] 异常:{ex.Message}");
                }

                if (!string.IsNullOrEmpty(json))
                    MessageHelper.SendMessage(title, json);
            });
        }
        /// <summary>
        /// 执行sql语句，返回json串
        /// </summary>
        /// <returns></returns>
        public static void OnExecuteSQLEvent(string sqlStr, string title)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                string json = string.Empty;
                try
                {
                    if (string.IsNullOrEmpty(sqlStr))
                        return;

                    IotHeader<DataTable> iot = new IotHeader<DataTable>();
                    iot.method = "execute";
                    iot.code = "200";
                    iot.data = ExecuteSql(sqlStr);
                    json = JsonConvert.SerializeObject(iot);
                }
                catch (Exception ex)
                {
                    IotHeader<string> error = new IotHeader<string>();
                    error.data = ex.Message;
                    error.method = "execute";
                    error.code = "400";
                    json = JsonConvert.SerializeObject(error);
                    LogHelper.logSoftWare.Debug($"[远程 OnExecuteSQLEvent] 异常:{ex.Message}");
                }

                if (!string.IsNullOrEmpty(json))
                    MessageHelper.SendMessage(title, json);
            });
        }

        private static DataTable ExecuteSql(string sql)
        {
            try
            {
                var dt = new DataTable();
                var providerName = ConfigurationManager.ConnectionStrings["DBConnectionStr"].ProviderName;
                var connectString = GetConnection(providerName);
                var factory = DbProviderFactories.GetFactory(providerName);
                using (var conn = factory.CreateConnection())
                {
                    conn.ConnectionString = connectString;
                    conn.Open();
                    var comm = conn.CreateCommand();
                    comm.CommandText = sql;
                    var adapter = factory.CreateDataAdapter();

                    adapter.SelectCommand = comm;
                    adapter.Fill(dt);
                }
                return dt;
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Debug($"[远程 ExecuteSql] SQL:{sql}, 异常:{ex.Message}");
                throw;
            }
        }

        private static string GetConnection(string ProviderName)
        {
            string str = ConfigurationManager.ConnectionStrings["DBConnectionStr"].ConnectionString.ToLower();
            if (ProviderName == "FirebirdSql.Data.FirebirdClient")
                return FirebirdConnection(str);

            return str;
        }

        /// <summary>
        /// 处理 Firebird 数据库连接字符串
        /// </summary>
        /// <param name="connectString"></param>
        /// <returns></returns>
        private static string FirebirdConnection(string connectString)
        {
            if (connectString.Contains("fdb") && (connectString.Contains("localhost") || connectString.Contains("127.0.0.1") || connectString.Contains("server type=1")))
            {
                if (connectString.Contains("database="))
                    connectString = connectString.Replace("database=", "database=" + MapPath.DataBasePath);
                else
                    connectString = connectString.Replace("initial catalog=", "initial catalog=" + MapPath.DataBasePath);

            }
            return connectString;
        }


        public static event DeviceEvent DeviceEvent;
        public static string OnDeviceEvent()
        {
            try
            {
                if (DeviceEvent != null)
                {
                    IotHeader<DeviceOptionsModel> iot = new IotHeader<DeviceOptionsModel>();
                    iot.method = "getDeviceOptions";
                    iot.data = new DeviceOptionsModel();
                    iot.data = DeviceEvent(iot);
                    iot.data.appPath = MapPath.AppDir;
                    var json = JsonConvert.SerializeObject(iot);
                    LogHelper.logSoftWare.Debug($"[远程 OnDeviceEvent ] 生成JSON：{JsonConvert.SerializeObject(iot)}");
                    return json;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                IotHeader<string> error = new IotHeader<string>();
                error.method = "getDeviceOptions";
                error.code = "400";
                error.data = ex.Message;
                var json = JsonConvert.SerializeObject(error);
                LogHelper.logSoftWare.Debug($"[远程 OnDeviceEvent] 异常:{ex.Message}");
                return json;
            }
        }

        public static event StatusEvent IsBusyEvent;
        /// <summary>
        /// 返回上位机状态，是否busy
        /// </summary>
        /// <returns></returns>
        public static string OnIsBusyEvent()
        {
            IotHeader<IsBusyStruct> iot = new IotHeader<IsBusyStruct>();
            iot.method = "isBusy";
            iot.data = new IsBusyStruct();
            try
            {
                if (IsBusyEvent == null)
                    return null;
                else
                    iot.data.isBusy = IsBusyEvent(iot);
            }
            catch (Exception ex)
            {
                iot.data.isBusy = false;
                iot.code = "400";
                LogHelper.logSoftWare.Debug($"[远程 OnIsBusyEvent] 异常:{ex.Message}");
            }

            var json = JsonConvert.SerializeObject(iot);
            LogHelper.logSoftWare.Debug($"[远程 OnIsBusyEvent ] 生成JSON：{JsonConvert.SerializeObject(iot)}");
            return json;
        }

        /// <summary>
        /// 注册上位机
        /// </summary>
        /// <param name="regInfo"></param>
        /// <returns></returns>
        public async static Task<Tuple<string, string>> Register(IotRegisterInfo regInfo)
        {
            try
            {
                ResetIotRegistetState();
                var iot = new IotRequestHeader<IotRegisterInfo>();
                iot.method = "register";
                iot.@params = new IotParams<IotRegisterInfo>();
                iot.@params.type = "diruiyun";
                iot.@params.value = regInfo;
                var json = JsonConvert.SerializeObject(iot);
                MessageHelper.SendMessage(targetTitle, json);
                LogHelper.logSoftWare.Debug($"[Iot Register ] 生成JSON：{json}");

                return await Task.Run(() =>
                {
                    if (regAutoResetEvent.WaitOne(1000 * 60))
                    {
                        return Tuple.Create(code, regCode);
                    }
                    else
                    {
                        LogHelper.logSoftWare.Debug($"[Iot Register ] 超时(60s)");
                        return Tuple.Create("-1", "");
                    }
                });
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Debug($"[Iot Register Exception] {ex.Message}");
                return await Task.Run(() => Tuple.Create("-2", ""));
            }
        }

        public static void IotRegisterCompleted(string code, string regCode)
        {
            IOTOperateManager.code = code;
            IOTOperateManager.regCode = regCode;
            regAutoResetEvent.Set();
        }

        private static void ResetIotRegistetState()
        {
            IOTOperateManager.code = string.Empty;
            IOTOperateManager.regCode = string.Empty;
            regAutoResetEvent.Reset();
        }

        #region 通用处理

        private static AutoResetEvent asyncSendInfoAutoResetEvent = new AutoResetEvent(false);
        private static string ReturnMethod = string.Empty;
        private static string ReturnCode = string.Empty;
        private static string ReturnInfo = string.Empty;
        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="regInfo"></param>
        /// <returns></returns>
        public async static Task<Tuple<string, string, string>> AsyncSendInfo(string methodName, object info)
        {
            try
            {
                ResetIotAsyncSendInfoState();
                var iot = new IotRequestHeader<object>();
                iot.method = methodName;
                iot.@params = new IotParams<object>();
                iot.@params.type = "diruiyun";
                iot.@params.value = info;
                var json = JsonConvert.SerializeObject(iot);
                MessageHelper.SendMessage(targetTitle, json);
                LogHelper.logSoftWare.Debug($"[Iot AsyncSendInfo ] 生成JSON：{json}");

                return await Task.Run(() =>
                {
                    if (asyncSendInfoAutoResetEvent.WaitOne(1000 * 60))
                    {
                        return Tuple.Create(ReturnMethod, ReturnCode, ReturnInfo);
                    }
                    else
                    {
                        LogHelper.logSoftWare.Debug($"[Iot AsyncSendInfo ] 超时(60s)");
                        return Tuple.Create("", "-1", "");
                    }
                });
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Debug($"[Iot AsyncSendInfo Exception] {ex.Message}");
                return await Task.Run(() => Tuple.Create("", "-2", ""));
            }
        }

        /// <summary>
        /// 线程阻塞，等待返回
        /// </summary>
        private static void ResetIotAsyncSendInfoState()
        {
            asyncSendInfoAutoResetEvent.Reset();
        }

        /// <summary>
        /// 远程返回结果
        /// </summary>
        /// <param name="code"></param>
        /// <param name="info"></param>
        public static void IotAsyncSendInfoCompleted(string method, string code, string info)
        {
            ReturnMethod = method;
            ReturnCode = code;
            ReturnInfo = info;
            asyncSendInfoAutoResetEvent.Set();
        }

        #endregion
    }

    /// <summary>
    /// 三元组信息
    /// </summary>
    public class DeviceOptionsModel
    {
        /// <summary>
        /// 产品线
        /// </summary>
        public string productKey { get; set; }
        /// <summary>
        /// 机型，例如：CS-2000
        /// </summary>
        public string deviceName { get; set; }
        /// <summary>
        /// 模块类型，例如：CS
        /// </summary>
        //public string ModuleType { get; set; }
        /// <summary>
        /// 模块编码，例如：1
        /// </summary>
        //public string ModuleCode { get; set; }
        /// <summary>
        /// 仪器编码
        /// </summary>
        public string clientId { get; set; }
        /// <summary>
        /// 上位机软件路径
        /// </summary>
        public string appPath { get; set; }

    }

    public enum IOTEnum
    {
        None,
        /// <summary>
        /// 查询三元组
        /// </summary>
        DeviceInfo,
        /// <summary>
        /// 设备忙
        /// </summary>
        Busy,
        /// <summary>
        /// 查询sql
        /// </summary>
        Sql
    }
    public enum IOTExecuteTypeEnum
    {
        None,
        /// <summary>
        /// 平台数据库
        /// </summary>
        Sql,
        /// <summary>
        /// 加密数据库
        /// </summary>
        EncryptDB,
        /// <summary>
        /// 文件
        /// </summary>
        File
    }
    /// <summary>
    /// 参数
    /// </summary>
    public class IOTEventArgs : EventArgs
    {
        /// <summary>
        /// IOT查询参数
        /// </summary>
        public string IOTPara { get; set; }
        /// <summary>
        /// IOT参数
        /// </summary>
        /// <param name="para">查询type</param>
        public IOTEventArgs(string para)
        {
            IOTPara = para;
        }
    }

    /// Iot响应头
    /// </summary>
    public class IotHeader<T>
    {
        /// <summary>
        /// 消息类型
        /// <para>
        /// getDeviceOptions 设备信息（三元组）申请
        /// executeSql SQL查询申请
        /// isBusy 设备是否忙查询申请
        /// </para>
        /// </summary>
        public string method { get; set; }

        /// <summary>
        /// 返回结果
        /// <para>
        /// 200 请求成功 
        /// 400 内部服务错误，处理时发生内部错误
        /// 460 请求参数错误，设备入参校验失败
        /// 429 请求过于频繁，设备端处理不过来时可以使用
        /// </para>
        /// </summary>
        public string code { get; set; } = "200";

        public T data { get; set; }
    }

    public class IotRequestHeader<T>
    {
        public string method { get; set; }

        public IotParams<T> @params { get; set; }
    }

    public class IotParams<T>
    {
        public string type { get; set; }

        public T value { get; set; }
    }

    public class IotRegisterInfo
    {
        /// <summary>
        /// 代理商
        /// </summary>
        public string agentName { get; set; }

        /// <summary>
        /// 医院地址
        /// </summary>
        public string hospitalAddr { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string hospitalPhone { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string memo { get; set; }

        /// <summary>
        /// 工程师
        /// </summary>
        public string saleName { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string salePhone { get; set; }

        /// <summary>
        /// 仪器ID
        /// </summary>
        public string machineID { get; set; }

        /// <summary>
        /// 电脑ID
        /// </summary>
        public string computerID { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public string offLineTime { get; set; }

        /// <summary>
        /// SN 码
        /// </summary>
        public string sn { get; set; }
    }

    public class IsBusyStruct
    {
        public bool isBusy { get; set; }
    }

    public class ExecuteStruct
    {
        public string ExecuteValue { get; set; }
    }
}
