using Sinboda.Framework.Common.DBOperateHelper;
using Sinboda.Framework.Common.FileOperateHelper;
using Sinboda.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace Sinboda.Framework.Control.Utils
{
    public class OperationLogBusiness
    {
        public static OperationLogBusiness Instance;
        /// <summary>
        /// 数据源
        /// </summary>
        string _DataSource = @"Data Source=";
        /// <summary>
        /// 程序路径
        /// </summary>
        string _Directory = MapPath.AppDir;
        /// <summary>
        /// 数据库
        /// </summary>
        string _FileName = @"Config\SysLog.db";
        /// <summary>
        /// db操作帮助类
        /// </summary>
        IDBHelper iDBHelper = new DBHelper(DBProvider.SQLite);

        string ConfigPath = MapPath.XmlPath + @"SOFTWAREINTERFACE_CONFIG.xml";

        bool isPrintLog = false;
        static OperationLogBusiness()
        {
            Instance = new OperationLogBusiness();
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public OperationLogBusiness()
        {
            string _ConnectString = Path.Combine(_Directory, _FileName);
            if (CreateDbFile(_ConnectString))
            {

            }
            ReadSetting();
        }
        private void ReadSetting()
        {
            try
            {
                XmlNode node = XMLHelper.GetXmlNodeByXpath(ConfigPath, "PrintSysLog");
                if (node != null && !string.IsNullOrEmpty(node.InnerText))
                {
                    isPrintLog = Convert.ToBoolean(node.InnerText);
                }
            }
            catch { }
        }
        public bool CreateLogTable()
        {
            string sql = @"CREATE TABLE oper_log(
                                                 id integer  NOT NULL primary key autoincrement,
	                                             user_name nvarchar(100) NULL,
                                                 create_time DATETIME NOT NULL,
                                                 message nvarchar(512) NULL,
	                                             types integer  NOT NULL
                                                )";
            int result = iDBHelper.ExcuteNonQueryInt(sql);
            if (result > 0)
                return true;
            else
                return false;
        }
        public bool WriteOperationLogToDb(string message, int types)
        {
            try
            {
                if (isPrintLog)
                {
                    if (iDBHelper != null)
                    {
                        string sql = @"insert into oper_log(user_name,create_time,message,types) 
                                            values ('{0}','{1}','{2}',{3})";
                        //List<DbParameter> values = new List<DbParameter>(); 
                        //values.Add(new DbParameter("user_name", userName)); 
                        //values.Add(new DbParameter("create_time", DateTime.Now));
                        //values.Add(new DbParameter("message", message));
                        //values.Add(new DbParameter("types", types));
                        sql = string.Format(sql, "", DateTime.Now.ToUniversalTime().ToString("s"), message, types);
                        int ormInt = iDBHelper.ExcuteNonQueryInt(sql);
                        if (ormInt > 0)
                        {
                            return true;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                //LogHelper.logSoftWare.Error(e.Message, e);
                //throw e;
            }
            return false;
        }

        public bool CreateDbFile(string dbPath)
        {
            try
            {
                if (!File.Exists(dbPath))
                {
                    File.Create(dbPath).Close();
                    bool result = iDBHelper.Init(_DataSource + dbPath);
                    if (result)
                    {
                        if (!CreateLogTable())
                        {
                            File.Delete(dbPath);
                            iDBHelper = null;
                            return false;
                        }
                    }
                    else
                    {
                        File.Delete(dbPath);
                        iDBHelper = null;
                        return false;
                    }

                }
                else
                {
                    bool result = iDBHelper.Init(_DataSource + dbPath);
                    if (!result)
                        iDBHelper = null;
                }
                return true;
            }
            catch (Exception ex)
            {
                //throw new Exception("新建数据库文件" + dbPath + "失败：" + ex.Message);
            }
            return false;
        }


    }

    /// <summary>
    /// 日志信息实体
    /// </summary>
    public class OperationLogModel
    {
        /// <summary>
        /// 类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Datetime { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        public string Message { get; set; }
    }
}
