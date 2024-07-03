using Sinboda.Framework.Core.StaticResource;
using Sinboda.SemiAuto.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Manager
{
    class ModuleVersionCacheManager
    {
    }
    /// <summary>
    /// 上位机版本号缓存处理类
    /// </summary>
    public class VirtualModuleCacheManager
    {
        /// <summary>
        /// 虚拟模块列表缓存
        /// </summary>
        public static Dictionary<int, string> InfoList { get; set; }

        /// <summary>
        /// 增加板号与名称对照关系
        /// </summary>
        public static void AddData()
        {
            InfoList = new Dictionary<int, string>();
            //InfoList.Add(0, "上位机");
            //InfoList.Add(1, "算法");
            InfoList.Add(0, SystemResources.Instance.LanguageArray[2804]);
            InfoList.Add(1, SystemResources.Instance.LanguageArray[8318]);
        }
    }

    /// <summary>
    /// 上位机版本号缓存处理类
    /// </summary>
    public class SoftWareVersionCacheManager
    {
        /// <summary>
        /// 上位机版本号列表缓存
        /// </summary>
        public static List<BoardInfo> InfoList { get; set; }

        /// <summary>
        /// 增加板号与名称对照关系
        /// </summary>
        public static void AddData()
        {
            InfoList = new List<BoardInfo>();
            //InfoList.Add(new BoardInfo() { ModuleId = 0, BoardId = 1, BoardName = "处理器编码", Version = string.Empty });
            //InfoList.Add(new BoardInfo() { ModuleId = 0, BoardId = 2, BoardName = "上位机版本", Version = string.Empty });
            //InfoList.Add(new BoardInfo() { ModuleId = 0, BoardId = 3, BoardName = "版权所有", Version = string.Empty });
            InfoList.Add(new BoardInfo() { ModuleId = 0, BoardId = 1, BoardName = SystemResources.Instance.GetLanguage(0, "处理器编码"), Version = string.Empty });
            InfoList.Add(new BoardInfo() { ModuleId = 0, BoardId = 2, BoardName = SystemResources.Instance.GetLanguage(0, "上位机版本"), Version = string.Empty });
            InfoList.Add(new BoardInfo() { ModuleId = 0, BoardId = 3, BoardName = SystemResources.Instance.GetLanguage(2118, "版权所有"), Version = string.Empty });
        }
    }
}
