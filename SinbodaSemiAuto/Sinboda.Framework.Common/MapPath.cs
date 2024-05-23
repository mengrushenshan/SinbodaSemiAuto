using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Common
{
    /// <summary>
    /// 程序根目录文件夹分布类
    /// </summary>
    public class MapPath
    {
        private static string appDir = AppDomain.CurrentDomain.BaseDirectory;
        /// <summary>
        /// 主程序根目录
        /// </summary>
        public static string AppDir
        {
            get { return appDir; }
            set { appDir = value; }
        }

        private static string configPath = appDir + "Config\\";
        /// <summary>
        /// Config文件夹目录：权限数据库、语言数据库、医院信息管理数据库
        /// </summary>
        public static string ConfigPath
        {
            get { return configPath; }
            set { configPath = value; }
        }

        private static string dataBasePath = appDir + "Data\\";
        /// <summary>
        /// DataBase数据库文件夹目录
        /// </summary>
        public static string DataBasePath
        {
            get { return dataBasePath; }
            set { dataBasePath = value; }
        }

        private static string helpPath = appDir + "Help\\";
        /// <summary>
        /// Help文件夹目录
        /// </summary>
        public static string HelpPath
        {
            get { return helpPath; }
            set { helpPath = value; }
        }

        private static string imagesPath = appDir + "Images\\";
        /// <summary>
        /// Images文件夹目录（程序使用显示的图标图片）
        /// </summary>
        public static string ImagesPath
        {
            get { return imagesPath; }
            set { imagesPath = value; }
        }

        private static string picturePath = appDir + "Picture\\";
        /// <summary>
        /// 图形文件夹目录（测试后产生的图形）
        /// </summary>
        public static string PicturePath
        {
            get { return picturePath; }
            set { picturePath = value; }
        }

        private static string logPath = appDir + "Log\\";
        /// <summary>
        /// 日志文件夹目录
        /// </summary>
        public static string LogPath
        {
            get { return logPath; }
            set { logPath = value; }
        }

        private static string originalDataPath = appDir + "OriginalData\\";
        /// <summary>
        /// 原始数据文件夹目录（样本结果、校准结果、质控结果分开存储）
        /// </summary>
        public static string OriginalDataPath
        {
            get { return originalDataPath; }
            set { originalDataPath = value; }
        }

        private static string printTempletPath = appDir + "PrintTemplate\\";
        /// <summary>
        /// 打印模版文件夹目录
        /// </summary>
        public static string PrintTempletPath
        {
            get { return printTempletPath; }
            set { printTempletPath = value; }
        }

        private static string updatePath = appDir + "Update\\";
        /// <summary>
        /// 升级数据库文件夹目录
        /// </summary>
        public static string UpdatePath
        {
            get { return updatePath; }
            set { updatePath = value; }
        }

        private static string xmlPath = appDir + "Xml\\";
        /// <summary>
        /// 配置文件夹目录
        /// </summary>
        public static string XmlPath
        {
            get { return xmlPath; }
            set { xmlPath = value; }
        }
    }
}
