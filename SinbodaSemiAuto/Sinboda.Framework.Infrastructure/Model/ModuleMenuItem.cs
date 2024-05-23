using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure.Model
{
    /// <summary>
    /// 模块类型
    /// </summary>
    public enum ModuleType
    {
        /// <summary>
        /// 不区分类型
        /// </summary>
        None,
        /// <summary>
        /// 生化类型
        /// </summary>
        CS,
        /// <summary>
        /// 发光类型
        /// </summary>
        CM,
        /// <summary>
        /// 凝血类型
        /// </summary>
        ACA,
        /// <summary>
        /// 尿分类型
        /// </summary>
        UM,
        /// <summary>
        /// 血球类型
        /// </summary>
        BF
    }

    /// <summary>
    /// 表示菜单项
    /// </summary>
    public class ModuleMenuItem
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 模块名
        /// </summary>
        public string ModuleName { get; set; }
        /// <summary>
        /// 类型名称
        /// </summary>
        public object Source { get; set; }
        /// <summary>
        /// 小图标
        /// </summary>
        public string Glyph { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsMenuShow { get; set; }
        /// <summary>
        /// 子菜单
        /// </summary>
        public List<ModuleMenuItem> ChildMenus { get; set; }
        /// <summary>
        /// 父节点
        /// </summary>
        public ModuleMenuItem ParentItem { get; set; }
        /// <summary>
        /// 模块类型
        /// </summary>
        public ModuleType ModuleType { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ModuleMenuItem()
        {
            ChildMenus = new List<ModuleMenuItem>();
        }
        /// <summary>
        /// 批量添加子项
        /// </summary>
        /// <param name="items"></param>
        public void AddItems(IEnumerable<ModuleMenuItem> items)
        {
            foreach (var item in items)
            {
                item.ParentItem = this;
                ChildMenus.Add(item);
            }
        }
    }
}
