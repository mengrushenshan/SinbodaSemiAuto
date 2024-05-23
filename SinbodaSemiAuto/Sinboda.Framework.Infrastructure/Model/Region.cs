using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure.Model
{
    /// <summary>
    /// 搜索范围
    /// </summary>
    public enum SearchRange
    {
        /// <summary>
        /// 模块 (默认值)
        /// <para>
        /// 搜索范围限制在已配置并且成功初始化的模块
        /// </para>
        /// </summary>
        Module,
        /// <summary>
        /// 全部
        /// <para>
        /// 搜索范围在所有已加载的程序集并且不考虑模块初始化情况
        /// </para>
        /// </summary>
        All
    }

    /// <summary>
    /// 表示区域的基本信息
    /// </summary>
    public class Region
    {
        /// <summary>
        /// 区域名称
        /// </summary>
        public string RegionName { get; internal set; }
        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; internal set; }
        /// <summary>
        /// 页面数据类型
        /// </summary>
        public string ViewType { get; internal set; }
        /// <summary>
        /// 页面实例
        /// </summary>
        public object View { get; set; }
        /// <summary>
        /// 搜索范围 (模块，全部)
        /// </summary>
        public SearchRange SearchRange { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="regionName"></param>
        /// <param name="moduleName"></param>
        /// <param name="viewType"></param>
        /// <param name="searchRange"></param>
        public Region(string regionName, string moduleName, string viewType, SearchRange searchRange)
        {
            RegionName = regionName;
            ModuleName = moduleName;
            ViewType = viewType;
            SearchRange = searchRange;
        }
    }
}
