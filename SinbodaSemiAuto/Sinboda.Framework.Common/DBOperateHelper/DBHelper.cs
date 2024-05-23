using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Sinboda.Framework.Common.Log;

namespace Sinboda.Framework.Common.DBOperateHelper
{
    /// <summary>
    /// 数据库操作配置信息
    /// </summary>
    public class DBConfig
    {
        /// <summary>
        /// 驱动名称
        /// </summary>
        public string ProviderName { set; get; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string Connection { set; get; }
    }
    /// <summary>
    /// 数据库帮助类
    /// </summary>
    public class DBHelper : IDBHelper
    {
        /// <summary>
        /// 错误处理
        /// </summary>
        public event Action<DBHelper, Exception> OnError;
        /// <summary>
        /// 静态构造函数
        /// </summary>
        static DBHelper()
        {
            _DbProviderDictionary = new Dictionary<DBProvider, string>();
            _DbProviderDictionary.Add(DBProvider.SqlServer, "System.Data.SqlClient");
            _DbProviderDictionary.Add(DBProvider.SQLite, "System.Data.SQLite");
            _DbProviderDictionary.Add(DBProvider.PostgreSQL, "Npgsql");
            _DbProviderDictionary.Add(DBProvider.MySQL, "MySql.Data.MySqlClient");
            _DbProviderDictionary.Add(DBProvider.FireBird, "FirebirdSql.Data.FirebirdClient");
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public DBHelper()
        {
            try
            {
                var providerName = ConfigurationManager.ConnectionStrings["DBConnectionStr"].ProviderName;
                var conn = DbProviderFactories.GetFactory(providerName).CreateConnection();
                var connectString = ConfigurationManager.ConnectionStrings["DBConnectionStr"].ConnectionString;
                conn.ConnectionString = connectString;
                _ProviderFactory = DbProviderFactories.GetFactory(providerName);
                Init(conn.ConnectionString);
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(null, e);
                throw e;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_providerName">数据库的枚举类型</param>
        public DBHelper(DBProvider _providerName)
        {
            try
            {
                _ProviderFactory = DbProviderFactories.GetFactory(_DbProviderDictionary[_providerName]);
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(null, e);
                throw e;
            }
        }
        /// <summary>
        /// 初始化构造函数(直接创建数据库连接,可注册 OnError获取错误事件)
        /// </summary>
        /// <param name="p_config">当传入配置属性为空时,读取当前应用程序app.config加载数据库配置</param>
        public DBHelper(DBConfig p_config)
        {
            string _constr = string.Empty;
            string _pname = string.Empty;
            try
            {
                if (p_config != null && !string.IsNullOrEmpty(p_config.Connection) && !string.IsNullOrEmpty(p_config.ProviderName))
                {
                    _constr = p_config.Connection;
                    _pname = p_config.ProviderName;
                }
                else
                {

                    var _config = ConfigurationManager.ConnectionStrings[1];//当配置文件
                    _constr = _config.ConnectionString;
                    _pname = _config.ProviderName;
                }
                switch (_pname)
                {
                    case "SqlServer":
                        _ProviderFactory = DbProviderFactories.GetFactory(_DbProviderDictionary[DBProvider.SqlServer]);
                        break;
                    case "SQLite"://sqllite只用相对路径及可  
                        _ProviderFactory = DbProviderFactories.GetFactory(_DbProviderDictionary[DBProvider.SQLite]);
                        break;
                    case "Npgsql":
                        _ProviderFactory = DbProviderFactories.GetFactory(_DbProviderDictionary[DBProvider.PostgreSQL]);
                        break;
                    case "MySql":
                        _ProviderFactory = DbProviderFactories.GetFactory(_DbProviderDictionary[DBProvider.MySQL]);
                        break;
                    case "FireBird":
                        _ProviderFactory = DbProviderFactories.GetFactory(_DbProviderDictionary[DBProvider.FireBird]);
                        break;
                }
                Init(_constr);
            }
            catch (Exception e)
            {
                if (OnError != null)
                {
                    LogHelper.logSoftWare.Error(null, e);
                    OnError(this, e);
                }
                throw;
            }
        }

        #region 私有变量
        /// <summary>
        /// .
        /// </summary>
        private static readonly Dictionary<DBProvider, string> _DbProviderDictionary;
        /// <summary>
        /// 
        /// </summary>
        private DbProviderFactory _ProviderFactory;
        /// <summary>
        /// 数据库连接
        /// </summary>
        private DbConnection _Conn;
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns>返回数据库连接</returns>
        private DbConnection GetConnection()
        {
            DbConnection _conn;
            _conn = this._ProviderFactory.CreateConnection();
            return _conn;
        }
        /// <summary>
        /// 获取相应数据库的适配器
        /// </summary>
        /// <returns>返回数据库适配器</returns>
        private DbDataAdapter GetDbDataAdapter()
        {
            DbDataAdapter _adp;
            _adp = this._ProviderFactory.CreateDataAdapter();
            return _adp;
        }
        /// <summary>
        /// 获取相应数据库的命令操作
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="strSQL">执行语句</param>
        /// <returns></returns>
        private DbCommand GetCommand(DbConnection conn, string strSQL)
        {
            DbCommand _command = conn.CreateCommand();
            _command.CommandText = strSQL;
            return _command;
        }
        #endregion

        #region 对外接口方法
        /// <summary>
        /// 初始化连接并打开
        /// </summary>
        /// <returns>操作返回类：成功为SUCCEED，Results为true；失败为FAILED，Results为false，Errors为异常。</returns>
        public bool Init(string connectionStr)
        {
            try
            {
                _Conn = GetConnection();
                _Conn.ConnectionString = connectionStr;
                return true;
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(null, e);
                throw e;
            }
            finally
            {
                if (_Conn != null)
                {
                    if (_Conn.State == ConnectionState.Open)
                    {
                        _Conn.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 执行SELECT查询语句，并返回操作结果。
        /// </summary>
        /// <param name="strSQL">需要执行的sql语句</param>
        /// <returns>操作返回类：成功为SUCCEED，Results为object；失败为FAILED，Results为null，Errors为异常。</returns>
        public object ExcuteQueryObject(string strSQL)
        {
            LogHelper.logSoftWareSQL.Debug("QObject " + strSQL);
            try
            {
                _Conn.Open();
                DbCommand comm = GetCommand(_Conn, strSQL);
                comm.CommandType = CommandType.Text;
                object obj = new object();
                obj = comm.ExecuteScalar();
                _Conn.Close();
                return obj;
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(null, e);
                throw e;
            }
            finally
            {
                if (_Conn != null)
                {
                    if (_Conn.State == ConnectionState.Open)
                    {
                        _Conn.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 执行SELECT查询语句，并返回操作结果。
        /// </summary>
        /// <param name="strSQL">需要执行的sql语句</param>
        /// <returns>操作返回类：成功为SUCCEED，Results为DataTable；失败为FAILED，Results为null，Errors为异常。</returns>
        public DataTable ExcuteQueryDataTable(string strSQL)
        {
            LogHelper.logSoftWareSQL.Debug("QTable " + strSQL);
            try
            {
                _Conn.Open();
                DbDataAdapter adp = GetDbDataAdapter();
                DbCommand comm = GetCommand(_Conn, strSQL);
                comm.CommandType = CommandType.Text;
                adp.SelectCommand = comm;
                DataTable dt = new DataTable();
                adp.Fill(dt);
                _Conn.Close();
                return dt;
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(null, e);
                throw e;
            }
            finally
            {
                if (_Conn != null)
                {
                    if (_Conn.State == ConnectionState.Open)
                    {
                        _Conn.Close();
                    }
                }
            }
        }
        public DataTable ExcuteQueryDataTable(string strSQL, List<DbParameter> parameters)
        {
            LogHelper.logSoftWareSQL.Debug("QTable " + strSQL);
            try
            {
                _Conn.Open();
                DbDataAdapter adp = GetDbDataAdapter();
                DbCommand comm = GetCommand(_Conn, strSQL);
                comm.CommandType = CommandType.Text;
                comm.Parameters.Clear();
                comm.Parameters.AddRange(parameters.ToArray());
                adp.SelectCommand = comm;
                DataTable dt = new DataTable();
                adp.Fill(dt);
                _Conn.Close();
                return dt;
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(null, e);
                throw e;
            }
            finally
            {
                if (_Conn != null)
                {
                    if (_Conn.State == ConnectionState.Open)
                    {
                        _Conn.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 执行存储过程，并返回操作结果。
        /// </summary>
        /// <param name="storeProcedureName">需要执行的存储过程名称</param>
        /// <returns>操作返回类：成功为SUCCEED，Results为DataTable；失败为FAILED，Results为null，Errors为异常。</returns>
        public DataTable ExcuteQueryDataTableProc(string storeProcedureName)
        {
            LogHelper.logSoftWareSQL.Debug("QTableByProc " + storeProcedureName);
            try
            {
                _Conn.Open();
                DbDataAdapter adp = GetDbDataAdapter();
                DbCommand comm = GetCommand(_Conn, storeProcedureName);
                comm.CommandType = CommandType.StoredProcedure;
                adp.SelectCommand = comm;
                DataTable dt = new DataTable();
                adp.Fill(dt);
                _Conn.Close();
                return dt;
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(null, e);
                throw e;
            }
            finally
            {
                if (_Conn != null)
                {
                    if (_Conn.State == ConnectionState.Open)
                    {
                        _Conn.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 执行SELECT查询语句，并返回操作结果。
        /// </summary>
        /// <param name="strSQL">需要执行的sql语句</param>
        /// <returns>操作返回类：成功为SUCCEED，Results为DataTable；失败为FAILED，Results为null，Errors为异常。</returns>
        public DbDataReader ExcuteQueryDataReader(string strSQL)
        {
            LogHelper.logSoftWareSQL.Debug("QReader " + strSQL);
            try
            {
                _Conn.Open();
                DbCommand _command = GetCommand(_Conn, strSQL);
                _command.CommandType = CommandType.Text;
                DbDataReader dr = _command.ExecuteReader();
                return dr;
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(null, e);
                throw e;
            }
        }

        /// <summary>
        /// 执行存储过程，并返回操作结果。
        /// </summary>
        /// <param name="storeProcedureName">需要执行的存储过程名称</param>
        /// <returns>操作返回类：成功为SUCCEED，Results为DataTable；失败为FAILED，Results为null，Errors为异常。</returns>
        public DbDataReader ExcuteQueryDataReaderProc(string storeProcedureName)
        {
            LogHelper.logSoftWareSQL.Debug("QReaderByProc " + storeProcedureName);
            try
            {
                _Conn.Open();
                DbCommand _command = GetCommand(_Conn, storeProcedureName);
                _command.CommandType = CommandType.StoredProcedure;
                DbDataReader dr = _command.ExecuteReader();
                return dr;
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(null, e);
                throw e;
            }
        }

        /// <summary>
        /// 执行非Select语句，包括UPDATE DELETE INSERT
        /// </summary>
        /// <param name="strSQL">需要执行的sql语句</param>
        /// <returns>操作返回类：成功为SUCCEED，Results为受影响的行数；失败为FAILED，Results为-1(意为失败)，Errors为异常</returns>
        public int ExcuteNonQueryInt(string strSQL)
        {
            LogHelper.logSoftWareSQL.Debug("NQ " + strSQL);
            try
            {
                _Conn.Open();
                DbCommand comm = GetCommand(_Conn, strSQL);
                comm.CommandType = CommandType.Text;
                int num = 0;
                num = comm.ExecuteNonQuery();
                _Conn.Close();
                return num;
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(null, e);
                throw e;
            }
            finally
            {
                if (_Conn != null)
                {
                    if (_Conn.State == ConnectionState.Open)
                    {
                        _Conn.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 执行非Select语句，包括UPDATE DELETE INSERT
        /// </summary>
        /// <param name="strSQL">需要执行的sql语句</param>
        /// <param name="parameters">参数集合</param>
        /// <returns>操作返回类：成功为SUCCEED，Results为受影响的行数；失败为FAILED，Results为-1(意为失败)，Errors为异常</returns>
        public int ExcuteNonQueryInt(string strSQL, List<DbParameter> parameters)
        {
            LogHelper.logSoftWareSQL.Debug("NQ " + strSQL);
            try
            {
                _Conn.Open();
                DbCommand comm = GetCommand(_Conn, strSQL);
                comm.CommandType = CommandType.Text;
                comm.Parameters.Clear();
                comm.Parameters.AddRange(parameters.ToArray());
                int num = 0;
                num = comm.ExecuteNonQuery();
                _Conn.Close();
                return num;
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(null, e);
                throw e;
            }
            finally
            {
                if (_Conn != null)
                {
                    if (_Conn.State == ConnectionState.Open)
                    {
                        _Conn.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 执行非Select语句，包括UPDATE DELETE INSERT
        /// </summary>
        /// <param name="storeProcedureName">需要执行的存储过程名称语句</param>
        /// <returns>操作返回类：成功为SUCCEED，Results为受影响的行数；失败为FAILED，Results为-1(意为失败)，Errors为异常</returns>
        public int ExcuteNonQueryIntProc(string storeProcedureName)
        {
            LogHelper.logSoftWareSQL.Debug("NQProc " + storeProcedureName);
            try
            {
                _Conn.Open();
                DbCommand comm = GetCommand(_Conn, storeProcedureName);
                comm.CommandType = CommandType.StoredProcedure;
                int num = 0;
                num = comm.ExecuteNonQuery();
                _Conn.Close();
                return num;
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(null, e);
                throw e;
            }
            finally
            {
                if (_Conn != null)
                {
                    if (_Conn.State == ConnectionState.Open)
                    {
                        _Conn.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 通过事务批量执行非查询SQL语句
        /// </summary>
        /// <param name="strSQLs">需要批量执行的SQL</param>
        /// <returns>操作返回类：成功为SUCCEED，Results为受影响的行数；失败为FAILED，Results为-1(意为回滚)，Errors为异常</returns>
        public int ExecuteNonQueryIntTransaction(List<string> strSQLs)
        {
            foreach (var n in strSQLs)
            {
                LogHelper.logSoftWareSQL.Debug("ListSQL " + n.ToString());
            }
            DbTransaction transaction = null;
            try
            {
                _Conn.Open();
                DbCommand comm = GetCommand(_Conn, "");
                int sumAffected = 0;
                transaction = _Conn.BeginTransaction();
                comm.Transaction = transaction;
                foreach (var n in strSQLs)
                {
                    comm.CommandText = n;
                    sumAffected += comm.ExecuteNonQuery();
                }
                transaction.Commit();
                _Conn.Close();
                return sumAffected;
            }
            catch (Exception e)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                    _Conn.Close();
                }
                LogHelper.logSoftWare.Error(null, e);
                throw e;
            }
            finally
            {
                if (_Conn != null)
                {
                    if (_Conn.State == ConnectionState.Open)
                    {
                        _Conn.Close();
                    }
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 数据库类型枚举
    /// </summary>
    public enum DBProvider
    {
        /// <summary>
        /// SQL Server.
        /// </summary>
        SqlServer,

        /// <summary>
        /// SQLite.
        /// </summary>
        SQLite,

        /// <summary>
        /// PostgreSQL.
        /// </summary>
        PostgreSQL,

        /// <summary>
        /// My SQL.
        /// </summary>
        MySQL,
        /// <summary>
        /// FireBird
        /// </summary>
        FireBird
    }
}
