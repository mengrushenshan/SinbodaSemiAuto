using Sinboda.Framework.Infrastructure.Interface;
using Sinboda.Framework.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sinboda.Framework.Infrastructure
{
    /// <summary>
    /// 表示程序启动类应具备的功能
    /// </summary>
    public interface IBootStrapper
    {
        /// <summary>
        /// 当前程序
        /// </summary>
        Application CurrentApp { get; }
        /// <summary>
        /// 初始化任务
        /// </summary>
        ICustomTaskManager<InitTaskResult> InitTaskManager { get; }
        /// <summary>
        /// 模块管理
        /// </summary>
        IModuleManager ModuleManager { get; }
        /// <summary>
        /// 程序关闭
        /// </summary>
        event EventHandler<AppCancelEventArgs> AppClosing;
        /// <summary>
        /// 用户切换
        /// </summary>
        event EventHandler<UserChangeEventArgs> UserChanging;

        /// <summary>
        /// 主窗口创建
        /// </summary>
        event EventHandler MianWindowCreated;

        /// <summary>
        /// 程序启动执行
        /// </summary>
        void Run();
        /// <summary>
        /// 添加模块信息
        /// </summary>
        /// <param name="moduleInfo"></param>
        void AddModuleInfo(ModuleInfo moduleInfo);
        /// <summary>
        /// 添加任务信息
        /// </summary>
        /// <param name="task"></param>
        /// <param name="index"></param>
        void AddInitTask(CustomTask<InitTaskResult> task, int index);
        /// <summary>
        /// 添加任务信息
        /// </summary>
        /// <param name="task"></param>
        void AddInitTask(CustomTask<InitTaskResult> task);
        /// <summary>
        /// 切换用户
        /// </summary>
        void ChangeUser(bool isAuto = false);
        /// <summary>
        /// 关闭程序
        /// </summary>
        bool Shutdown();
        /// <summary>
        /// 直接关闭程序
        /// </summary>
        bool ShutdownCore();

        void WindowCreated();

        /// <summary>
        /// 设置默认页面名称
        /// </summary>
        void SetDefaultView(string viewName);
    }

    #region EventArgs

    /// <summary>
    /// 取消操作事件
    /// </summary>
    public class AppCancelEventArgs : EventArgs
    {
        /// <summary>
        /// 是否取消
        /// </summary>
        public bool Cancel { get; set; }
    }

    /// <summary>
    /// 用户切换事件
    /// </summary>
    public class UserChangeEventArgs : EventArgs
    {
        /// <summary>
        /// 是否取消
        /// </summary>
        public bool Cancel { get; set; }
        /// <summary>
        /// 是否是自动动作
        /// </summary>
        public bool IsAutoAction { get; set; }
    }
    #endregion
}
