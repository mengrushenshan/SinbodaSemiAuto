using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.AbstractClass
{
    /// <summary>
    /// 表示一个 <see cref="DbContext"/> 实例的作用范围
    /// </summary>
    public class ContextScope<T> : IDisposable where T : DbContext, new()
    {
        /// <summary>
        /// 在 <seealso cref="ContextScope"/> 作用域中唯一的 <seealso cref="DbContext"/> 实例
        /// </summary>
        [ThreadStatic]
        private static T context;

        /// <summary>
        /// 返回当前实例的父级 <seealso cref="ContextScope{T}"/> 实例
        /// </summary>
        internal ContextScope<T> Parent { get; private set; }

        /// <summary>
        /// 获取 <seealso cref="DbContext"/> 实例
        /// </summary>
        public T Context
        {
            get { return context; }
            private set { context = value; }
        }

        /// <summary>
        /// 创建一个 <seealso cref="ContextScope{T}"/> 的实例
        /// </summary>
        internal ContextScope() { }

        /// <summary>
        /// 创建一个 <seealso cref="ContextScope{T}"/> 的实例
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="parent">父级 <seealso cref="ContextScope{T}"/> 实例</param>
        internal ContextScope(T dbContext, ContextScope<T> parent = null)
        {
            if (parent == null)
                Context = dbContext;
            else
            {
                Context = parent.Context;
                Parent = parent;
            }
        }

        /// <summary>
        /// 完成操作，提交变更到数据库
        /// </summary>
        public void Complete()
        {
            if (Parent == null)
                Context.SaveChanges();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (Parent == null)
            {
                Context.Dispose();
                Context = null;
            }
        }
    }
}
