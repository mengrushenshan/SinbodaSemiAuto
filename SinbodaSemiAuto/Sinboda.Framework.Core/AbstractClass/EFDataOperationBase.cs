using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.AbstractClass
{
    /// <summary>
    /// 基于 EntityFramework 的数据层操作基类
    /// </summary>
    /// <typeparam name="TDataOperation">DataOperation 类型</typeparam>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TDbContext">DbContext类型</typeparam>
    [Serializable]
    public abstract class EFDataOperationBase<TDataOperation, TEntity, TDbContext> : TBaseSingleton<TDataOperation>
        where TEntity : EntityModelBase
        where TDataOperation : new()
        where TDbContext : DbContext, new()
    {
        /// <summary>
        /// 创建数据库上下文
        /// </summary>
        /// <param name="parent">数据库使用的上下文</param>
        /// <returns></returns>
        public ContextScope<TDbContext> CreateContextScope(ContextScope<TDbContext> parent = null)
        {
            return new ContextScope<TDbContext>(new TDbContext(), parent);
        }
        /// <summary>
        /// 创建一个空的 <seealso cref="Comparer{T}"/> 实例
        /// </summary>
        /// <returns></returns>
        private ContextScope<TDbContext> CreateNullContextScope()
        {
            return new ContextScope<TDbContext>();
        }

        /// <summary>
        /// 按主键查询
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public virtual TEntity Find(object primaryKey)
        {
            var cs = CreateNullContextScope();
            var db = cs.Context ?? new TDbContext();

            return db.Set<TEntity>().Find(primaryKey);
        }
        /// <summary>
        /// 按条件查询数据，按 Create_time 倒序排列
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        public virtual List<TEntity> Query(Expression<Func<TEntity, bool>> predicate)
        {
            var cs = CreateNullContextScope();
            var db = cs.Context ?? new TDbContext();
            try
            {
                return db.Set<TEntity>().AsNoTracking().Where(predicate).OrderByDescending(o => o.Create_time).ToList();
            }
            finally
            {
                if (cs.Context == null)
                    db.Dispose();
            }
        }
        /// <summary>
        /// 按条件查询，返回符合条件的记录数
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        public virtual int QueryCount(Expression<Func<TEntity, bool>> predicate)
        {
            var cs = CreateNullContextScope();
            var db = cs.Context ?? new TDbContext();
            try
            {
                int result = db.Set<TEntity>().AsNoTracking().Where(predicate).Count();
                return result;
            }
            finally
            {
                if (cs.Context == null)
                {
                    db.SaveChanges();
                    db.Dispose();
                }
            }
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            var cs = CreateNullContextScope();
            var db = cs.Context ?? new TDbContext();

            return db.Set<TEntity>().AsNoTracking().Where(predicate);
        }
        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Insert(TEntity entity)
        {
            var cs = CreateNullContextScope();
            var db = cs.Context ?? new TDbContext();
            try
            {
                if (entity.Id == null || entity.Id == Guid.Empty)
                    entity.Id = Guid.NewGuid();
                entity.Create_time = DateTime.Now;
                entity.Create_user = SystemResources.Instance.CurrentUserName;
                db.Set<TEntity>().Add(entity);
            }
            finally
            {
                if (cs.Context == null)
                {
                    db.SaveChanges();
                    db.Dispose();
                }
            }
        }
        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="entitys"></param>
        public virtual void Insert(List<TEntity> entitys)
        {
            var cs = CreateNullContextScope();
            var db = cs.Context ?? new TDbContext();
            try
            {
                foreach (var entity in entitys)
                {
                    if (entity.Id == null || entity.Id == Guid.Empty)
                        entity.Id = Guid.NewGuid();

                    // 未设置 Create_time 时，使用当前系统时间
                    if (entity.Create_time == default(DateTime))
                        entity.Create_time = DateTime.Now;

                    entity.Create_user = SystemResources.Instance.CurrentUserName;
                    db.Set<TEntity>().Add(entity);
                }
            }
            finally
            {
                if (cs.Context == null)
                {
                    db.SaveChanges();
                    db.Dispose();
                }
            }
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(TEntity entity)
        {
            var cs = CreateNullContextScope();
            var db = cs.Context ?? new TDbContext();

            try
            {
                if (db.Entry(entity).State == EntityState.Detached)
                    db.Set<TEntity>().Attach(entity);

                entity.Create_user = SystemResources.Instance.CurrentUserName;
                db.Entry(entity).State = EntityState.Modified;
            }
            finally
            {
                if (cs.Context == null)
                {
                    db.SaveChanges();
                    db.Dispose();
                }
            }
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entitys"></param>
        public virtual void Update(List<TEntity> entitys)
        {
            var cs = CreateNullContextScope();
            var db = cs.Context ?? new TDbContext();

            try
            {
                foreach (var entity in entitys)
                {
                    if (db.Entry(entity).State == EntityState.Detached)
                        db.Set<TEntity>().Attach(entity);

                    entity.Create_user = SystemResources.Instance.CurrentUserName;
                    db.Entry(entity).State = EntityState.Modified;
                }
            }
            finally
            {
                if (cs.Context == null)
                {
                    db.SaveChanges();
                    db.Dispose();
                }
            }
        }
        /// <summary>
        /// 全部删除
        /// </summary>
        public virtual void DeleteAll()
        {
            var cs = CreateNullContextScope();
            var db = cs.Context ?? new TDbContext();

            try
            {
                var list = db.Set<TEntity>().ToList();
                db.Set<TEntity>().RemoveRange(list);
            }
            finally
            {
                if (cs.Context == null)
                {
                    db.SaveChanges();
                    db.Dispose();
                }
            }
        }

        /// <summary>
        /// 按条件删除
        /// </summary>
        /// <param name="predicate"></param>
        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var cs = CreateNullContextScope();
            var db = cs.Context ?? new TDbContext();

            try
            {
                var list = db.Set<TEntity>().Where(predicate);
                db.Set<TEntity>().RemoveRange(list);
            }
            finally
            {
                if (cs.Context == null)
                {
                    db.SaveChanges();
                    db.Dispose();
                }
            }
        }
        /// <summary>
        /// 按条件删除
        /// </summary>
        /// <param name="predicate"></param>
        public virtual void Delete(object primaryKey)
        {
            var cs = CreateNullContextScope();
            var db = cs.Context ?? new TDbContext();

            try
            {
                var result = db.Set<TEntity>().Find(primaryKey);
                db.Set<TEntity>().Remove(result);
            }
            finally
            {
                if (cs.Context == null)
                {
                    db.SaveChanges();
                    db.Dispose();
                }
            }
        }

        /// <summary>
        /// 执行非查询单个sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual int ExecuteSqlCommand(string sql)
        {
            var cs = CreateNullContextScope();
            var db = cs.Context ?? new TDbContext();
            try
            {
                var result = db.Database.ExecuteSqlCommand(sql);
                return result;
            }
            finally
            {
                if (cs.Context == null)
                {
                    db.Dispose();
                }
            }
        }
        /// <summary>
        /// 执行非查询多个sql语句
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        public virtual int ExecuteSqlCommand(List<string> sqls)
        {
            var cs = CreateNullContextScope();
            var db = cs.Context ?? new TDbContext();

            var tran = db.Database.BeginTransaction();
            try
            {
                int result = 0;
                foreach (var sql in sqls)
                    result += db.Database.ExecuteSqlCommand(sql);
                tran.Commit();
                return result;
            }
            catch (Exception e)
            {
                tran.Rollback();
                LogHelper.logSoftWare.Error("ExecuteSqlCommand error", e);
                throw e;
            }
            finally
            {
                if (cs.Context == null)
                {
                    db.Dispose();
                }
            }
        }
        /// <summary>
        /// 执行非查询单个sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            var cs = CreateNullContextScope();
            var db = cs.Context ?? new TDbContext();
            try
            {
                var result = db.Database.ExecuteSqlCommand(sql, parameters);
                return result;
            }
            finally
            {
                if (cs.Context == null)
                {
                    db.Dispose();
                }
            }
        }
        /// <summary>
        /// 执行单个sql查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public IList<TEntity> ExecuteSqlQuery<TEntity>(string commandText)
        {
            var cs = CreateNullContextScope();
            var db = cs.Context ?? new TDbContext();

            var result = db.Database.SqlQuery<TEntity>(commandText).ToList();
            return result;
        }
        /// <summary>
        /// 执行单个sql查询，带有参数
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IList<TEntity> ExecuteSqlQuery<TEntity>(string commandText, params object[] parameters)
        {
            var cs = CreateNullContextScope();
            var db = cs.Context ?? new TDbContext();

            var result = db.Database.SqlQuery<TEntity>(commandText, parameters).ToList();
            return result;
        }
        /// <summary>
        /// 执行存储过程查询，带有参数
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IList<TEntity> ExecuteStoredProcedureWithOutput<TEntity>(string commandText, params object[] parameters)
        {
            var cs = CreateNullContextScope();
            var db = cs.Context ?? new TDbContext();

            if (parameters != null && parameters.Length > 0)
            {
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    var p = parameters[i] as DbParameter;
                    if (p == null)
                        throw new Exception("Not support parameter type");
                    commandText += i == 0 ? "" : ", ";
                    commandText += "@" + p.ParameterName;
                    if (p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output)
                    {
                        commandText += " output";
                    }
                }
            }

            var result = db.Database.SqlQuery<TEntity>(commandText, parameters).ToList();
            return result;
        }
    }
}
