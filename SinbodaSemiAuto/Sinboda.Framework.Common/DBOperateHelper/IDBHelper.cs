using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Common.DBOperateHelper
{
    /// <summary>
    /// 数据库帮助类接口
    /// </summary>
    public interface IDBHelper
    {
        /// <summary>
        /// 初始化连接并打开
        /// </summary>
        /// <returns> 成功为true；失败为false </returns>
        bool Init(string connectionStr);

        /// <summary>
        /// 执行SELECT查询语句，并返回操作结果。
        /// </summary>
        /// <param name="strSQL">需要执行的sql语句</param>
        /// <returns>查询结果</returns>
        object ExcuteQueryObject(string strSQL);

        /// <summary>
        /// 执行SELECT查询语句，并返回操作结果。
        /// </summary>
        /// <param name="strSQL">
        /// 需要执行的sql语句
        /// </param>
        /// <returns> 查询结果 </returns>
        DataTable ExcuteQueryDataTable(string strSQL);
        /// <summary>
        /// 执行SELECT查询语句，带查询参数，并返回操作结果。
        /// </summary>
        /// <param name="strSQL">需要执行的sql语句</param>
        /// <param name="parameters">参数集合</param>
        /// <returns></returns>
        DataTable ExcuteQueryDataTable(string strSQL, List<DbParameter> parameters);

        /// <summary>
        /// 执行存储过程，并返回操作结果。
        /// </summary>
        /// <param name="storeProcedureName">
        /// 需要执行的存储过程名称
        /// </param>
        /// <returns> 查询结果 </returns>
        DataTable ExcuteQueryDataTableProc(string storeProcedureName);

        /// <summary>
        /// 执行SELECT查询语句，并返回操作结果。
        /// </summary>
        /// <param name="strSQL">
        /// 需要执行的sql语句
        /// </param>
        /// <returns> 查询结果 </returns>
        DbDataReader ExcuteQueryDataReader(string strSQL);

        /// <summary>
        /// 执行存储过程，并返回操作结果。
        /// </summary>
        /// <param name="storeProcedureName">
        /// 需要执行的存储过程名称
        /// </param>
        /// <returns> 查询结果 </returns>
        DbDataReader ExcuteQueryDataReaderProc(string storeProcedureName);

        /// <summary>
        /// 执行非Select语句，包括UPDATE DELETE INSERT
        /// </summary>
        /// <param name="strSQL">
        /// 需要执行的sql语句
        /// </param>
        /// <returns> 受影响的行数 </returns>
        int ExcuteNonQueryInt(string strSQL);
        /// <summary>
        /// 执行非Select语句，包括UPDATE DELETE INSERT
        /// </summary>
        /// <param name="strSQL">需要执行的sql语句</param>
        /// <param name="parameters">参数集合</param>
        /// <returns></returns>
        int ExcuteNonQueryInt(string strSQL, List<DbParameter> parameters);

        /// <summary>
        /// 执行非Select语句，包括UPDATE DELETE INSERT
        /// </summary>
        /// <param name="storeProcedureName">
        /// 需要执行的sql语句
        /// </param>
        /// <returns> 受影响的行数 </returns>
        int ExcuteNonQueryIntProc(string storeProcedureName);

        /// <summary>
        /// 通过事务批量执行非查询SQL语句
        /// </summary>
        /// <param name="strSQLs">
        /// 需要批量执行的SQL
        /// </param>
        /// <returns> 受影响的行数 </returns>
        int ExecuteNonQueryIntTransaction(List<string> strSQLs);
    }
}
