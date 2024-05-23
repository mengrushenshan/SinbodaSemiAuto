using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.Services
{
    /// <summary>
    /// 数据库实例化及EntityFramework预热处理
    /// </summary>
    public class DbContextInitializeService : IDbContextInitialize
    {
        /// <summary>
        /// 
        /// </summary>
        public DbContextInitializeService()
        {
            DatabaseType = typeof(DBContextBase);
        }

        /// <summary>
        /// 数据库上下文类型
        /// </summary>
        public Type DatabaseType { get; set; }

        /// <summary>
        /// 如果数据库存在忽略EntityFramework自己的数据结构比较，实现方式为<see cref="Database.SetInitializer{T}(null);"/>，T为继承自<see cref="DBContextBase"/>的实现类
        /// </summary>
        public void IfExistIgnoreCreate()
        {
            using (var db = new DBContextBase())
            {
                Database.SetInitializer<DBContextBase>(null);
            }
        }
        /// <summary>
        /// 数据库创建
        /// </summary>
        public void InitializeDB()
        {
            //数据库实例
            using (DBContextBase db = new DBContextBase())
            {
                db.Database.Create();
            }
        }
        /// <summary>
        /// 数据库初始化数据
        /// </summary>
        public void InitializeData()
        {

        }
    }
}
