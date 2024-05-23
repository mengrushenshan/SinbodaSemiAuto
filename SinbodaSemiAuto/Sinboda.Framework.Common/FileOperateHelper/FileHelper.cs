using Sinboda.Framework.Common.DBOperateHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Sinboda.Framework.Common.FileOperateHelper
{
    /// <summary>
    /// 文件处理帮助类
    /// </summary>
    public class FileHelper : IFileHelper
    {
        #region  继承接口实现类

        /// <summary>
        /// xml文件保存
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="p_entity">传入实体</param>
        /// <param name="p_savePath">保存路径和文件名</param>
        /// <returns>是否保存成功</returns>

        public bool SaveXML<T>(T p_entity, string p_savePath) where T : class
        {
            return SerializeHelper.FileXmlSerialize(p_entity, p_savePath);
        }

        /// <summary>
        /// xml文件读取
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="p_readPath">读取路径和文件名</param>
        /// <returns>返回XML文档实体类实例</returns>
        public T ReadXML<T>(string p_readPath) where T : class
        {
            return SerializeHelper.FileXmlDeserialize<T>(p_readPath);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="nodePath"></param>
        /// <param name="xmlNodeNameAttrName"></param>
        /// <param name="xmlNodeNameAttrValue"></param>
        /// <returns></returns>
        public XmlNodeList XMLGetXmlNodeByName(string xmlFileName, string nodePath, string xmlNodeNameAttrName, string xmlNodeNameAttrValue)
        {
            return XMLHelper.GetXMLChildNodeList(xmlFileName, nodePath, xmlNodeNameAttrName, xmlNodeNameAttrValue);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="xpath"></param>
        /// <param name="xmlNodeName"></param>
        /// <param name="xmlNodeNameAttrName"></param>
        /// <param name="xmlNodeNameAttrValue"></param>
        /// <param name="sourceNode"></param>
        /// <returns></returns>
        public bool XMLUpdateXmlNodeByName(string xmlFileName, string xpath, string xmlNodeName, string xmlNodeNameAttrName, string xmlNodeNameAttrValue, XmlNode sourceNode)
        {
            return XMLHelper.UpdateXMLChildNodeList(xmlFileName, xpath, xmlNodeName, xmlNodeNameAttrName, xmlNodeNameAttrValue, sourceNode);
        }

        /// <summary>
        /// 删除INI节点
        /// </summary>
        /// <param name="section">区域符</param>
        /// <param name="key">键</param>
        /// <param name="path">文件路径</param>
        public void DeleteINI(string section, string key, string path)
        {
            INIHelper.Delete(section, key, path);
        }

        /// <summary>
        /// 保存INI节点
        /// </summary>
        /// <param name="section">区域符</param>
        /// <param name="key">键</param>
        /// <param name="iValue">值</param>
        /// <param name="path">文件路径</param>
        public void SaveINI(string section, string key, string iValue, string path)
        {
            INIHelper.Write(section, key, iValue, path);
        }

        /// <summary>
        /// 读取INI节点
        /// </summary>
        /// <param name="section">区域符</param>
        /// <param name="key">键</param>
        /// <param name="path">文件路径</param>
        /// <returns>读取到的节点信息</returns>
        public string ReadINI(string section, string key, string path)
        {
            return INIHelper.Read(section, key, path);
        }

        /// <summary>
        /// 保存TXT
        /// </summary>
        /// <param name="p_savePath">保存路径</param>
        /// <param name="p_contents">文件内容</param>
        public void SaveTXT(string p_savePath, string p_contents)
        {
            WriteAllText(p_savePath, p_contents);
        }

        /// <summary>
        /// 读取Txt文件
        /// </summary>
        /// <param name="p_readPath">读取路径</param>
        /// <returns></returns>
        public string ReadTXT(string p_readPath)
        {
            return ReadAllText(p_readPath);
        }

        /// <summary>
        /// 删除txt文件
        /// </summary>
        /// <param name="p_delPath">删除路径</param>
        /// <returns></returns>
        public bool DeleteTXT(string p_delPath)
        {
            return DeleteFile(p_delPath);
        }

        #endregion

        #region 文件常用操作

        /// <summary>
        /// 获取文件路径中最后的目录名
        /// </summary>
        /// <param name="p_fullName"></param>
        /// <returns></returns>
        public static string GetDirectoryName(string p_fullName)
        {
            if (string.IsNullOrWhiteSpace(p_fullName))
            {
                return null;
            }
            return p_fullName.Substring(0, p_fullName.LastIndexOf('\\') + 1);
        }

        /// <summary>
        /// 获取路径中的文件名称
        /// </summary>
        /// <param name="p_filePath"></param>
        /// <returns></returns>
        public static string GetFileName(string p_filePath)
        {
            if (string.IsNullOrWhiteSpace(p_filePath))
            {
                return string.Empty;
            }
            if (p_filePath.Length > 260)
            {
                return p_filePath.Substring(p_filePath.LastIndexOf('\\') + 1, int.MaxValue);
            }
            return Path.GetFileName(p_filePath);
        }

        /// <summary>
        /// 文件名是否满足filePattern格式。
        /// </summary>
        /// <param name="p_fileName"></param>
        /// <param name="p_filePattern"></param>
        public static bool IsMatched(string p_fileName, string p_filePattern)
        {
            if (string.IsNullOrWhiteSpace(p_fileName))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(p_filePattern))
            {
                return false;
            }
            Regex regex = new Regex(p_filePattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(p_fileName);
        }

        /// <summary>
        /// 写txt文件
        /// </summary>
        /// <param name="p_filePath">写入路径</param>
        /// <param name="p_contents">写入内容</param>
        public static void WriteAllText(string p_filePath, string p_contents)
        {
            if (string.IsNullOrEmpty(p_contents.Trim()))
            {
                return;
            }
            File.WriteAllText(p_filePath, p_contents);
        }

        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="p_filePath">读取路径</param>
        /// <returns></returns>
        public static string ReadAllText(string p_filePath)
        {
            if (string.IsNullOrWhiteSpace(p_filePath) || File.Exists(p_filePath) == false)
            {
                return string.Empty;
            }
            return File.ReadAllText(p_filePath);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="p_filePath"></param>
        public static bool DeleteFile(string p_filePath)
        {
            if (string.IsNullOrWhiteSpace(p_filePath))
            {
                return false;
            }
            if (!File.Exists(p_filePath))
            {
                return false;
            }
            File.Delete(p_filePath);
            return !File.Exists(p_filePath);
        }

        /// <summary>
        /// 删除目录下所有过期文件
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="expiredDays"></param>
        public static int ClearExpiredFiles(string directory, int expiredDays)
        {
            if (!Directory.Exists(directory))
            {
                return 0;
            }
            if (expiredDays <= 0)
            {
                return 0;
            }
            DirectoryInfo dir = new DirectoryInfo(directory);
            IList<FileInfo> fileInfos = dir.GetFiles();
            if (fileInfos == null || fileInfos.Count < 1)
            {
                return 0;
            }
            int count = 0;
            foreach (FileInfo fileInfo in fileInfos)
            {
                if (fileInfo.CreationTime.AddDays(expiredDays) < DateTime.Now)
                {
                    //added by yangbinggang
                    fileInfo.Attributes = FileAttributes.Normal;
                    FileHelper.DeleteFile(fileInfo.FullName);
                    count = count + 1;
                }
            }
            return count;
        }

        /// <summary>
        /// 删除目录下所有过期文件
        /// </summary>
        /// <param name="p_dirs">目录数组</param>
        /// <param name="p_expiredDays">过期天数</param>
        /// <returns></returns>
        public static int ClearExpiredFiles(string[] p_dirs, int p_expiredDays)
        {
            if (p_dirs == null)
            {
                return 0;
            }
            if (p_dirs.Length <= 0)
            {
                return 0;
            }
            int count = 0;
            foreach (string dir in p_dirs)
            {
                count = count + ClearExpiredFiles(dir, p_expiredDays);
            }
            return count;
        }

        /// <summary>
        /// 删除过期目录及其子目录和文件
        /// </summary>
        /// <param name="p_directories">目录数组</param>
        /// <param name="p_expiredDays"></param>
        /// <returns></returns>
        public static int ClearExpiredDirectories(string[] p_directories, int p_expiredDays)
        {
            if (p_directories == null || p_directories.Length <= 0)
            {
                return 0;
            }
            if (p_expiredDays < 0)
            {
                return 0;
            }
            int count = 0;
            foreach (string directory in p_directories)
            {
                if (!Directory.Exists(directory))
                {
                    continue;
                }
                count += ClearExpiredFiles(directory, p_expiredDays);
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                if (directoryInfo.CreationTime.AddDays(p_expiredDays) < DateTime.Now)
                {
                    directoryInfo.Attributes = FileAttributes.Normal;
                    Directory.Delete(directory, true);
                }
            }
            return count;
        }

        /// <summary>
        /// 深度枚举出所有子目录（包括子目录的子目录）
        /// </summary>
        /// <param name="p_directory"></param>
        /// <returns></returns>
        public static IList<string> EnumerateAllSubDirectories(string p_directory)
        {
            List<string> direcotryList = new List<string>();
            if (string.IsNullOrWhiteSpace(p_directory))
            {
                return direcotryList;
            }
            if (!Directory.Exists(p_directory))
            {
                return direcotryList;
            }

            string[] folders = Directory.GetDirectories(p_directory);
            direcotryList.AddRange(folders);
            foreach (string folder in folders)
            {
                direcotryList.AddRange(EnumerateAllSubDirectories(folder));
            }
            return direcotryList;
        }

        /// <summary>
        /// 根据时间查询文件
        /// </summary>
        /// <param name="p_directory"></param>
        /// <param name="p_maxCount"></param>
        /// <param name="p_days"></param>
        /// <returns></returns>
        public static IList<string> FindFiles(string p_directory, int p_maxCount, int p_days)
        {
            IList<string> fileList = new List<string>();
            if (!Directory.Exists(p_directory) || p_maxCount <= 0)
            {
                return fileList;
            }
            string[] files = Directory.GetFiles(p_directory);
            if (files == null)
            {
                return fileList;
            }
            DateTime lastTime = DateTime.Now.AddDays(-Math.Abs(p_days));
            fileList = files.Where(item =>
            {
                if (p_maxCount <= 0)
                {
                    return false;
                }
                FileInfo fileInfo = new FileInfo(item);
                if (fileInfo.LastWriteTime >= lastTime)
                {
                    p_maxCount--;
                    return true;
                }
                return false;
            }).ToList<string>();
            return fileList;
        }

        /// <summary>
        /// 查询目录下的所有文件，将recursive设为True可查询子目录中的所有文件。
        /// </summary>
        /// <param name="p_directory"></param>
        /// <param name="p_filePattern"></param>
        /// <param name="p_maxCount"></param>
        /// <param name="p_recursive"></param>
        /// <returns></returns>
        public static IList<FileInfo> FindFiles(string p_directory, string p_filePattern, int p_maxCount, bool p_recursive)
        {
            if (!p_recursive)
            {
                return FileHelper.FindFiles(p_directory, p_filePattern, p_maxCount);
            }
            IList<string> directories = EnumerateAllSubDirectories(p_directory);
            return FindFiles(directories, p_filePattern, p_maxCount);
        }
        private static IList<FileInfo> FindFiles(IList<string> directories, string filePattern, int maxCount)
        {
            List<FileInfo> files = new List<FileInfo>();
            foreach (string directoryItem in directories)
            {
                files.AddRange(FileHelper.FindFiles(directoryItem, filePattern, maxCount));
                if (files.Count > maxCount)
                {
                    break;
                }
            }
            return files.GetRange(0, Math.Min(files.Count, maxCount));
        }

        /// <summary>
        /// 默认查找20个文件
        /// </summary>
        /// <param name="p_directory">文件路径</param>
        /// <param name="p_filePattern">文件格式</param>
        /// <returns></returns>
        public static IList<FileInfo> FindFiles(string p_directory, string p_filePattern)
        {
            int maxCount = 20;
            return FindFiles(p_directory, p_filePattern, maxCount);
        }

        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="p_directory">文件路径</param>
        /// <param name="p_filePattern">文件格式</param>
        /// <param name="p_maxCount">最大数量</param>
        /// <returns></returns>
        public static IList<FileInfo> FindFiles(string p_directory, string p_filePattern, int p_maxCount)
        {
            List<FileInfo> matchedFiles = new List<FileInfo>();
            IList<FileInfo> fileInfos = FindAllFiles(p_directory);
            if (string.IsNullOrWhiteSpace(p_filePattern))
            {
                return matchedFiles;
            }
            if (p_maxCount < 0 || p_maxCount > fileInfos.Count)
            {
                p_maxCount = fileInfos.Count;
            }
            p_maxCount--;
            foreach (var fileInfo in fileInfos)
            {
                if (p_maxCount < 0)
                {
                    break;
                }
                if (FileHelper.IsMatched(fileInfo.Name, p_filePattern))
                {
                    matchedFiles.Add(fileInfo);
                }
                p_maxCount--;
            }
            return matchedFiles;
        }

        /// <summary>
        ///查找所有文件
        /// </summary>
        /// <param name="p_directory">文件路径</param>
        /// <returns></returns>
        public static IList<FileInfo> FindAllFiles(string p_directory)
        {
            IList<FileInfo> fileInfos = new List<FileInfo>();
            if (string.IsNullOrWhiteSpace(p_directory))
            {
                return fileInfos;
            }
            if (!Directory.Exists(p_directory))
            {
                return fileInfos;
            }
            DirectoryInfo dir = new DirectoryInfo(p_directory);
            fileInfos = dir.GetFiles();
            return fileInfos;
        }

        /// <summary>
        /// 重新生成新的文件路径
        /// </summary>
        /// <param name="p_filePath">文件路径</param>
        /// <returns></returns>
        public static string Rename(string p_filePath)
        {
            if (string.IsNullOrWhiteSpace(p_filePath))
            {
                return string.Empty;
            }
            string lastFileName = Path.GetFileNameWithoutExtension(p_filePath);
            string lastFileExtension = Path.GetExtension(p_filePath);
            //重命名，则随机在原来文件名后面加几个随机数字进行组装成新的名字
            Random random = new Random(System.DateTime.Now.Millisecond);
            string randomData = random.Next().ToString();
            //把原文件名的名字加上随机数，组装成新的文件名（避免重名）
            string newFileName = lastFileName + randomData;
            return Path.GetDirectoryName(p_filePath) + "\\" + newFileName + lastFileExtension;
        }
        #endregion
    }
}
