using Sinboda.Framework.Common.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Helpers
{
    public class IOService
    {
        public static string DocumentRead(string path)
        {
            string nTemp = "";
            try
            {
                StreamReader sr = new StreamReader(path);
                nTemp = sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error("DocumentRead error:" + ex.ToString());
            }
            return nTemp;
        }
        public static bool DocumentWrite(string path, string aStr)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(aStr);
                        sw.Flush();
                        sw.Close();
                        fs.Close();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error("DocumentWrite error" + ex.ToString());
                return false;
            }
        }
        public static void DeleteFile(string spath)
        {
            try
            {
                string sFileName, sPath;
                //sPath = @"D:\CAN";
                sPath = spath;
                DirectoryInfo folder = new DirectoryInfo(sPath);
                DirectoryInfo[] a11 = folder.GetDirectories();
                foreach (DirectoryInfo file in folder.GetDirectories())
                {
                    string path = file.FullName;
                    DirectoryInfo fo = new DirectoryInfo(path);
                    FileInfo[] aaaa = fo.GetFiles("*.log");
                    foreach (FileInfo fi in fo.GetFiles("*.log"))
                    {
                        string name = fi.Name.Split('.')[0];
                        name = name.Replace("-", "");
                        string dt = DateTime.Now.ToString("yyyyMMdd");
                        int a = int.Parse(dt) - int.Parse(name);
                        if (a > 10)
                        {
                            File.Delete(fi.FullName);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error("DeleteFile error" + ex.ToString());
            }


        }
        /// <summary>
        /// 复制文件夹及文件
        /// </summary>
        /// <param name="sourceFolder">原文件路径</param>
        /// <param name="destFolder">目标文件路径</param>
        /// <returns></returns>
        public static int CopyFolder(string sourceFolder, string destFolder)
        {
            try
            {
                //如果目标路径不存在,则创建目标路径
                if (!System.IO.Directory.Exists(destFolder))
                {
                    System.IO.Directory.CreateDirectory(destFolder);
                }
                //得到原文件根目录下的所有文件
                string[] files = System.IO.Directory.GetFiles(sourceFolder);
                foreach (string file in files)
                {
                    string name = System.IO.Path.GetFileName(file);
                    string dest = System.IO.Path.Combine(destFolder, name);
                    System.IO.File.Copy(file, dest);//复制文件
                }
                //得到原文件根目录下的所有文件夹
                string[] folders = System.IO.Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders)
                {
                    string name = System.IO.Path.GetFileName(folder);
                    string dest = System.IO.Path.Combine(destFolder, name);
                    CopyFolder(folder, dest);//构建目标路径,递归复制文件
                }
                return 1;
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("CopyFolder error" + e.ToString());
                return 0;
            }

        }
    }
}
