using Sinboda.Framework.Common.DBOperateHelper;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Common;
using Sinboda.Framework.Core.CommonModels;
using Sinboda.Framework.Core.Interface;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Sinboda.Framework.Core.ModelsOperation
{
    /// <summary>
    /// 
    /// </summary>
    public class AnalyzerInfoOperations : IAnalyzerInfo
    {
        /// <summary>
        /// 数据源
        /// </summary>
        string _DataSource = @"Data Source=";
        /// <summary>
        /// 程序路径
        /// </summary>
        string _Directory = MapPath.AppDir;
        /// <summary>
        /// 数据库
        /// </summary>
        string _FileName = @"Config\\PerUsers.db";
        /// <summary>
        /// db操作帮助类
        /// </summary>
        IDBHelper iDBHelper = new DBHelper(DBProvider.SQLite);
        /// <summary>
        /// 构造函数
        /// </summary>
        public AnalyzerInfoOperations()
        {
            string _ConnectString = Path.Combine(_Directory, _FileName);
            bool result = iDBHelper.Init(_DataSource + _ConnectString);
            if (!result)
                iDBHelper = null;
        }
        /// <summary>
        /// 获取信息
        /// </summary>
        public CurrentModuleInfo GetAnalyzerInfo()
        {
            string sql = string.Empty;
            sql = "select * from AnalyzerInfo";
            DataTable table = iDBHelper.ExcuteQueryDataTable(sql);
            CurrentModuleInfo info = new CurrentModuleInfo();
            info.CompanyLogoPath = table.Rows[0]["CompanyLogo"].ToString();
            BitmapImage image = new BitmapImage();
            if (File.Exists(MapPath.ImagesPath + info.CompanyLogoPath))
            {
                image.BeginInit();
                image.UriSource = new Uri(MapPath.ImagesPath + info.CompanyLogoPath, UriKind.RelativeOrAbsolute);
                image.EndInit();
            }
            info.CompanyLogo = image;
            info.AnalyzerType = table.Rows[0]["AnalyzerType"].ToString();
            info.LanguageID = int.Parse(table.Rows[0]["LanguageID"].ToString());
            info.AnalyzerName = info.LanguageID != 0 ? SystemResources.Instance.LanguageArray[info.LanguageID] : table.Rows[0]["AnalyzerName"].ToString();
            SystemResources.Instance.AnalyzerInfoLogo = image;
            SystemResources.Instance.AnalyzerInfoTypeName = table.Rows[0]["AnalyzerType"].ToString();
            SystemResources.Instance.AnalyzerInfoName = info.LanguageID != 0 ? SystemResources.Instance.LanguageArray[info.LanguageID] : table.Rows[0]["AnalyzerName"].ToString();
            return info;
        }

        /// <summary>
        /// 更新信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool SetAnalyzerInfo(CurrentModuleInfo info)
        {
            string sql = string.Empty;
            sql = string.Format("update AnalyzerInfo set CompanyLogo='{0}',AnalyzerName='{1}',AnalyzerType='{2}',LanguageID='{3}'",
                                info.CompanyLogoPath, info.AnalyzerName, info.AnalyzerType, info.LanguageID);
            int result = iDBHelper.ExcuteNonQueryInt(sql);
            if (result > 0)
                return true;
            else return false;
        }

        static byte[] BitmapToBytes(BitmapImage bmp)
        {
            byte[] result = null;
            try
            {
                Stream stream = bmp.StreamSource;
                if (stream != null && stream.Length > 0)
                {
                    stream.Position = 0;
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        result = br.ReadBytes((int)stream.Length);
                    }
                }
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("BitmapToBytes", e);
                throw e;
            }

            return result;
        }

        static BitmapImage ByteToBitmapImage(byte[] bytes)
        {
            BitmapImage bmp = null;
            try
            {
                bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(bytes);
                bmp.EndInit();
            }
            catch (System.Exception e)
            {
                bmp = null;
                LogHelper.logSoftWare.Error("ByteToBitmapImage", e);
                throw e;
            }
            return bmp;
        }
    }
}
