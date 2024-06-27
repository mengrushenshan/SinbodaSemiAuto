using System.Collections.Generic;
using Python.Runtime;
using System.IO;

namespace Sinboda.SemiAuto.Core.Helpers
{
    public static class PyHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Init()
        {
            string pathToVirtualEnv = ".\\Python310";
            Runtime.PythonDLL = Path.Combine(pathToVirtualEnv, "python310.dll");
            PythonEngine.PythonHome = Path.Combine(pathToVirtualEnv, "python.exe");
            PythonEngine.PythonPath = $"./scripts;{pathToVirtualEnv}/DLLs;{pathToVirtualEnv}/Lib;{pathToVirtualEnv}/Lib/site-packages;";
            PythonEngine.Initialize();
        }

        /// <summary>
        /// 自动计算焦点
        /// </summary>
        /// <param name="tifPath">图片地址</param>
        /// <returns>两种计算方式的计算结果 图片游标</returns>
        public static List<int> Autofocus(string tifPath)
        {
            List<int> list = new List<int>();
            using (Py.GIL())
            {
                dynamic py = Py.Import("auto_focus");
                PyList res = PyList.AsList(py.find_focused_img_tif(tifPath));
                //PyList res = PyList.AsList(py.find_focused_img_pngs("E:/scripts/png_folder"));
                list.Add((int)(dynamic)res[0]);
                list.Add((int)(dynamic)res[1]);
            }
            return list;
        }

        /// <summary>
        /// 分析荧光点数量
        /// </summary>
        /// <param name="tifPath">图片地址</param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static int DataAnalyze(string tifPath, int row, int col)
        {
            int cellNum = -1;
            using (Py.GIL())
            {
                dynamic py = Py.Import("data_analyzer");
                PyList res = PyList.AsList(py.analyze_single(tifPath, row, col));
                cellNum = (int)(dynamic)res[1];
            }
            return cellNum;
        }

        public static void Shutdown()
        {
            PythonEngine.Shutdown();
        }
    }
}
