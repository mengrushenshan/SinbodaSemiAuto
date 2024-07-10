using System.Collections.Generic;
using Python.Runtime;
using System.IO;

namespace Sinboda.SemiAuto.Core.Helpers
{
    public static class PyHelper
    {
        private static bool flgInit = false;
        /// <summary>
        /// 
        /// </summary>
        public static void Init()
        {
            if (flgInit)
                return;

            string pathToVirtualEnv = ".\\Python310";
            Runtime.PythonDLL = Path.Combine(pathToVirtualEnv, "python310.dll");
            PythonEngine.PythonHome = Path.Combine(pathToVirtualEnv, "python.exe");
            PythonEngine.PythonPath = $"./scripts;{pathToVirtualEnv}/DLLs;{pathToVirtualEnv}/Lib;{pathToVirtualEnv}/Lib/site-packages;";

            flgInit = true;
        }

        /// <summary>
        /// 自动计算焦点
        /// </summary>
        /// <param name="tifPath">图片地址</param>
        /// <returns>两种计算方式的计算结果 图片游标</returns>
        public static List<int> Autofocus(string tifPath)
        {
            List<int> list = new List<int>();
            if (flgInit)
            {
                PythonEngine.Initialize();
                using (Py.GIL())
                {
                    dynamic pyAutoFocus = Py.Import("auto_focus");
                    PyList res = PyList.AsList(pyAutoFocus.find_focused_img_tif(tifPath));
                    //PyList res = PyList.AsList(py.find_focused_img_pngs("E:/scripts/png_folder"));
                    list.Add((int)(dynamic)res[0]);
                    list.Add((int)(dynamic)res[1]);
                }
                PythonEngine.Shutdown();
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
            if (flgInit)
            {
                PythonEngine.Initialize();
                using (Py.GIL())
                {
                    dynamic pyDataAnalyzer = Py.Import("data_analyzer");
                    PyList res = PyList.AsList(pyDataAnalyzer.analyze_single(tifPath, row, col));
                    cellNum = (int)(dynamic)res[1];
                }
                PythonEngine.Shutdown();
            }
            return cellNum;
        }

        /// <summary>
        /// 分析荧光点数量
        /// </summary>
        /// <param name="tifPath">图片地址</param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static int DataAnalyzeImageJ(string tifPath, int row, int col)
        {
            int cellNum = -1;
            string currPath = System.AppDomain.CurrentDomain.BaseDirectory;
            string imageJ_path  = Path.Combine(currPath, "Fiji.App");

            if (flgInit)
            {
                PythonEngine.Initialize();
                using (Py.GIL())
                {
                    dynamic pyDataAnalyzer = Py.Import("data_analyzer_imageJ");
                    cellNum = (int)PyInt.AsInt(pyDataAnalyzer.analyze_single(tifPath, (char)row, col, imageJ_path));
                }
                PythonEngine.Shutdown();
            }
            return cellNum;
        }
    }
}
