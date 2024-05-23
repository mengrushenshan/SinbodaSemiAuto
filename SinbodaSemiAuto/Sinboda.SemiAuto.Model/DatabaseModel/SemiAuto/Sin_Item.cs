using Sinboda.Framework.Core.AbstractClass;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto
{
    /// <summary>
    /// 检查项目主表   保存测试项目的一些基础信息
    /// </summary>
    [Serializable]
    public partial class Sin_Item : EntityModelBase
    {
        private int item_id;
        /// <summary>
        /// 项目编号
        /// </summary>
        public int Item_id
        {
            get { return item_id; }
            set { Set(ref item_id, value); }
        }

        private string itemName;
        /// <summary>
        /// 检测项目
        /// </summary>
        public string ItemName
        {
            get { return itemName; }
            set { Set(ref itemName, value); }
        }

        private string full_name;
        /// <summary>
        /// 项目全称
        /// </summary>
        public string Full_name
        {
            get { return full_name; }
            set { Set(ref full_name, value); }
        }

        private string langid;
        public string LangID
        {
            get { return langid; }
            set { Set(ref langid, value); }
        }

        /// <summary>
        /// 测试项目类型
        /// </summary>
        public ItemType Item_type { get; set; }

        private string barcode_code;
        /// <summary>
        /// 条码号
        /// </summary>
        public string Barcode_code
        {
            get { return barcode_code; }
            set { Set(ref barcode_code, value); }
        }

        private int print_code;
        /// <summary>
        /// 打印编号
        /// </summary>
        public int Print_code
        {
            get { return print_code; }
            set { Set(ref print_code, value); }
        }

        /// <summary>
        /// Lis 编号
        /// </summary>
        public string Lis_code { get; set; }

        /// <summary>
        /// 测试顺序
        /// </summary>
        public int Test_order { get; set; }

        /// <summary>
        /// 项目是否启动
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 项目参数是否可用或完整。 参数完整才可发送测试
        /// </summary>
        public bool Param_enabled { get; set; }

        /// <summary>
        /// 是否是校准项目
        /// </summary>
        public bool Item_is_calibrated { get; set; }
    }

}
