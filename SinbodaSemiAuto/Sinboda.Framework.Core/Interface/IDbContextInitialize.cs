using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.Interface
{
    /// <summary>
    /// 数据库初始化接口
    /// </summary>
    public interface IDbContextInitialize
    {
        /// <summary>
        /// 继承自<see cref="DBContextBase"/>的实现类
        /// </summary>
        Type DatabaseType { get; set; }
        /// <summary>
        /// 如果数据库存在忽略EntityFramework自己的数据结构比较，实现方式为<see cref="Database.SetInitializer{T}(null);"/>，T为继承自<see cref="DBContextBase"/>的实现类
        /// </summary>
        void IfExistIgnoreCreate();
        /// <summary>
        /// 数据库创建
        /// </summary>
        void InitializeDB();
        /// <summary>
        /// 数据库初始化数据
        /// </summary>
        void InitializeData();
    }
}
