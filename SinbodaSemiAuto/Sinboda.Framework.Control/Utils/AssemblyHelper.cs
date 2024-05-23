using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Control.Utils
{
    internal static class AssemblyHelper
    {
        private static Assembly entryAssembly;
        /// <summary>
        /// 获取可执行文件的程序集
        /// </summary>
        public static Assembly EntryAssembly
        {
            get
            {
                if (entryAssembly == null)
                    entryAssembly = Assembly.GetEntryAssembly();
                return entryAssembly;
            }
            set
            {
                entryAssembly = value;
            }
        }


        /// <summary>
        /// 获取当前应用程序域已加载的<see cref="Assembly"/>集合
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetLoadedAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}
