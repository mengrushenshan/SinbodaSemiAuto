using System.Collections.Generic;

namespace Sinboda.Framework.Common.ExportImportHelper
{
    /// <summary>
    /// 导出结构类
    /// </summary>
    /// <typeparam name="T">任意引用类型</typeparam>
    public class ExportData<T> where T : class
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportData()
        {
            Datas = new List<T>();
            PropertiesToColumnHeads = new Dictionary<string, string>();
        }

        /// <summary>
        /// 需要导出的数据集合（注意多国语言，必填）
        /// </summary>
        public List<T> Datas { get; set; }

        /// <summary>
        /// Excel的Sheet页名（注意多国语言）
        /// </summary>
        public string SheetName { get; set; }

        /// <summary>
        /// 导出时类中属性和Excel中列名的对应关系（key-属性名称  value-列名，必填）
        /// </summary>

        public Dictionary<string, string> PropertiesToColumnHeads { get; set; }

    }

    /// <summary>
    /// 主子表导出结构
    /// </summary>
    /// <typeparam name="MasterT"></typeparam>
    /// <typeparam name="DetailT"></typeparam>
    public class ExportMasterDetailData<MasterT, DetailT> where MasterT : class where DetailT : class
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportMasterDetailData()
        {
            MasterData = new List<MasterT>();
            MasterPropertiesToColumnHeads = new Dictionary<string, string>();
            DetailPropertiesToColumnHeads = new Dictionary<string, string>();
        }

        /// <summary>
        /// 主表数据
        /// </summary>
        public List<MasterT> MasterData { get; set; }

        /// <summary>
        /// Excel的Sheet页名（注意多国语言）
        /// </summary>
        public string SheetName { get; set; }

        /// <summary>
        /// 子表属性
        /// </summary>
        public string DetailProperty { get; set; }

        /// <summary>
        /// 导出时 主表中属性和Excel中列名的对应关系（key-属性名称  value-列名，必填）
        /// </summary>
        public Dictionary<string, string> MasterPropertiesToColumnHeads { get; set; }

        /// <summary>
        /// 导出时 子表中属性和Excel中列名的对应关系（key-属性名称  value-列名，必填）
        /// </summary>
        public Dictionary<string, string> DetailPropertiesToColumnHeads { get; set; }
    }

    /// <summary>
    /// 导入导出函数接口
    /// </summary>
    public interface IExportAndImport
    {
        /// <summary>
        /// 单个集合数据导出到Excel文件中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">预导出文件名（含路径）</param>
        /// <param name="exportDatas">数据源</param>
        /// <returns></returns>
        bool ExportFile<T>(string fileName, ExportData<T> exportDatas) where T : class;

        /// <summary>
        /// 两个集合数据导出到同一个Excel文件中
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="fileName">预导出文件名（含路径）</param>
        /// <param name="exportDatas1">数据源</param>
        /// <param name="exportDatas2">数据源</param>
        /// <returns></returns>
        bool ExportFile<T1, T2>(string fileName, ExportData<T1> exportDatas1, ExportData<T2> exportDatas2) where T1 : class where T2 : class;

        /// <summary>
        /// 三个集合数据导出到同一个Excel文件中
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="fileName">预导出文件名（含路径）</param>
        /// <param name="exportDatas1">数据源</param>
        /// <param name="exportDatas2">数据源</param>
        /// <param name="exportDatas3">数据源</param>
        /// <returns></returns>
        bool ExportFile<T1, T2, T3>(string fileName, ExportData<T1> exportDatas1, ExportData<T2> exportDatas2, ExportData<T3> exportDatas3) where T1 : class where T2 : class where T3 : class;

        /// <summary>
        /// 单个集合（主子表）导出到Excel文件中
        /// </summary>
        /// <typeparam name="MasterT"></typeparam>
        /// <typeparam name="DetailT"></typeparam>
        /// <param name="fileName">预导出文件名（含路径）</param>
        /// <param name="exportDatas">数据源</param>
        /// <returns></returns>
        bool ExportFile<MasterT, DetailT>(string fileName, ExportMasterDetailData<MasterT, DetailT> exportDatas) where MasterT : class where DetailT : class;

        /// <summary>
        /// DataSet导出到指定文件中
        /// </summary>
        /// <param name="filename">预导出文件名（含路径）</param>
        /// <param name="srcData">数据源</param>
        /// <returns>false-失败 ture-成功</returns>
        bool ExportFile(string filename, System.Data.DataSet srcData);

        /// <summary>
        /// 数据导入功能
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <returns>DataSet结果不为空-成功，DataSet结果为null-失败</returns>
        System.Data.DataSet ImportFromFile(string filename);
    }
}
