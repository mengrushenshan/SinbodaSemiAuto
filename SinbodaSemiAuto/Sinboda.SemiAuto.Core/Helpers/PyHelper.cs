using System.Collections.Generic;
using Python.Runtime;
using System.IO;

namespace Sinboda.SemiAuto.Core.Helpers
{
    public static class PyHelper
    {
        public static void Init()
        {
            string pathToVirtualEnv = ".\\Python310";
            Runtime.PythonDLL = Path.Combine(pathToVirtualEnv, "python310.dll");
            PythonEngine.PythonHome = Path.Combine(pathToVirtualEnv, "python.exe");
            PythonEngine.PythonPath = $"./scripts;{pathToVirtualEnv}/DLLs;{pathToVirtualEnv}/Lib;{pathToVirtualEnv}/Lib/site-packages;";
            PythonEngine.Initialize();
        }

        public static List<int> Autofocus()
        {
            List<int> list = new List<int>();
            using (Py.GIL())
            {
                dynamic py = Py.Import("auto_focus");
                PyList res = PyList.AsList(py.find_focused_img("png_folder", "E:/scripts/png_folder"));
                list.Add((int)(dynamic)res[0]);
                list.Add((int)(dynamic)res[1]);
            }
            return list;
        }

        public static void Shutdown()
        { 
            PythonEngine.Shutdown();
        }
    }
}
