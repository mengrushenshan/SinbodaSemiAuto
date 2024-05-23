using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure
{
    /// <summary>
    /// 程序启动类
    /// </summary>
    public class BootStrapper
    {
        private static IBootStrapper current;
        /// <summary>
        /// 
        /// </summary>
        public static IBootStrapper Current
        {
            get { return current; }
            private set { current = value; }
        }

        public static List<string> ChildProcessList = new List<string>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bootStrapper"></param>
        /// <returns></returns>
        public static IBootStrapper RegisterBootStrapper(IBootStrapper bootStrapper)
        {
            return Current = bootStrapper;
        }

        /// <summary>
        /// 记录所有子进程名称，方便退出时退出所有子进程
        /// </summary>
        /// <param name="processNameList"></param>
        public static void RegisterChildProcess(List<string> processNameList)
        {
            ChildProcessList.Clear();
            if (null != processNameList)
            {
                ChildProcessList.AddRange(processNameList);
            }
        }

        private BootStrapper() { }
    }
}
