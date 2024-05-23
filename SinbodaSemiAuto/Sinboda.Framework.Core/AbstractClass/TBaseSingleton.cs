using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.AbstractClass
{
    /// <summary>
    /// 系统单例模式基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TBaseSingleton<T> where T : new()
    {
        private static readonly T instance = new T();
        /// <summary>
        /// 
        /// </summary>
        public static T Instance
        {
            get
            {
                return instance;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected TBaseSingleton()
        {

        }
    }
}
