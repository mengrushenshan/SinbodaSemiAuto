using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.Interface
{
    /// <summary>
    /// 窗口接口
    /// </summary>
    public interface IWindow
    {
        /// <summary>
        /// 初始化控件
        /// </summary>
        void InitializeComponent();
        /// <summary>
        /// 打开窗口
        /// </summary>
        void ShowWindow();
        /// <summary>
        /// 关闭窗口
        /// </summary>
        void CloseWindow();
        /// <summary>
        /// 加载数据
        /// </summary>
        void LoadDatas();
        /// <summary>
        /// 刷新数据
        /// </summary>
        void RefreshDatas();
    }
}
