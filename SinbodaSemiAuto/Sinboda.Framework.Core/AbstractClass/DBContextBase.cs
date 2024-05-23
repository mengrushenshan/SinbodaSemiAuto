using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Common;
using Sinboda.Framework.Core.BusinessModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Sinboda.Framework.Core.BusinessModels.Mapping;
using System.Data.SQLite;

namespace Sinboda.Framework.Core.AbstractClass
{
    /// <summary>
    /// 数据库操作基类
    /// </summary>
    public class DBContextBase : DbContext
    {
        /// <summary>
        /// 当前使用连接字符串，默认使用pg
        /// </summary>
        public static string connectStr = "DBConnectionStr";
        /// <summary>
        /// 构造函数
        /// </summary>
        public DBContextBase() : base(connectStr)
        {
            try
            {
                TransferFileDatabaseConnStr();
                WriteDatabaseLog();
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("DBContextBase error", e);
                throw;
            }
        }

        /// <summary>
        /// 更换文件数据库连接字符串
        /// </summary>
        protected virtual void TransferFileDatabaseConnStr()
        {
            //这是为了处理当前连接字符串中给定的是绝对路径
            if (Database.Connection.ConnectionString.ToLower().Contains(":\\"))
                return;

            //这是为了处理当前连接字符串中给定的是相对路径，而firebird用的是绝对路径，所以需要替换成全部路径
            if (Database.Connection.ConnectionString.ToLower().Contains("fdb") &&
                (Database.Connection.ConnectionString.ToLower().Contains("localhost") || Database.Connection.ConnectionString.Contains("127.0.0.1") || Database.Connection.ConnectionString.ToLower().Contains("server type=1")))
                if (Database.Connection.ConnectionString.Contains("DataBase="))
                    Database.Connection.ConnectionString = Database.Connection.ConnectionString.Replace("DataBase=", "DataBase=" + MapPath.DataBasePath);
                else
                    Database.Connection.ConnectionString = Database.Connection.ConnectionString.Replace("initial catalog=", "initial catalog=" + MapPath.DataBasePath);
        }

        /// <summary>
        /// 记录数据库log日志
        /// </summary>
        protected virtual void WriteDatabaseLog()
        {
            Database.Log = a =>
            {
                if (a.ToString().Contains("Opened connection"))
                    LogHelper.logSoftWareSQL.Debug("---------------------------begin-----------------------------------\r\n");
                LogHelper.logSoftWareSQL.Debug(a);
                if (a.ToString().Contains("Closed connection"))
                    LogHelper.logSoftWareSQL.Debug("---------------------------end-----------------------------------\r\n");
            };
        }

        /// <summary>
        /// 版本信息表
        /// </summary>
        public DbSet<VersionModel> VersionModel { get; set; }

        /// <summary>
        /// 报警历史信息实体
        /// </summary>
        public DbSet<AlarmHistoryInfoModel> AlarmHistoryInfoModel { get; set; }

        /// <summary>
        /// 系统日志信息实体
        /// </summary>
        public DbSet<SysLogModel> SysLogModel { get; set; }
        /// <summary>
        /// 系统注册信息实体
        /// </summary>
        public DbSet<SysRegisterModel> SysRegisterModel { get; set; }
        /// <summary>
        /// 基础信息类型表
        /// </summary>
        public DbSet<DataDictionaryTypeModel> DataDictionaryTypeModel { get; set; }
        /// <summary>
        /// 基础信息信息表
        /// </summary>
        public DbSet<DataDictionaryInfoModel> DataDictionaryInfoModel { get; set; }


        /// <summary>
        /// 模块类型表
        /// </summary>
        public DbSet<ModuleTypeModel> ModuleTypeModel { get; set; }
        /// <summary>
        /// 模块信息表
        /// </summary>
        public DbSet<ModuleInfoModel> ModuleInfoModel { get; set; }

        /// <summary>
        /// 各模块版本信息
        /// </summary>
        public DbSet<ModuleVersionModel> ModuleVersionModel { get; set; }

        /// <summary>
        /// 数据库表中字段部署关系
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new SysLogModelMap());
            modelBuilder.Configurations.Add(new AlarmHistoryInfoModelMap());
            modelBuilder.Configurations.Add(new DataDictionaryTypeModelMap());
            modelBuilder.Configurations.Add(new DataDictionaryInfoModelMap());
            modelBuilder.Configurations.Add(new ModuleTypeModelMap());
            modelBuilder.Configurations.Add(new ModuleInfoModelMap());
            modelBuilder.Configurations.Add(new VersionModelMap());
            modelBuilder.Configurations.Add(new ModuleVersionModelMap());
            modelBuilder.Configurations.Add(new SysRegisterModelMap());

        }
    }

    public class SystemValueContext : DbContext
    {
        /// <summary>
        /// 系统基础信息类型表
        /// </summary>
        public DbSet<SystemTypeModel> SystemTypeModel { get; set; }
        /// <summary>
        /// 系统基础信息信息表
        /// </summary>
        public DbSet<SystemTypeValueModel> SystemTypeValueModel { get; set; }

        public SystemValueContext()
            : base(new SQLiteConnection("Data Source=Data\\SystemValue.db;BinaryGUID=False"), false)
        {
            Database.Log = a =>
            {
                Debug.WriteLine(a);
            };
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new SystemTypeModelMap());
            modelBuilder.Configurations.Add(new SystemTypeValueModelMap());
        }
    }
}
