using Newtonsoft.Json;
using Sinboda.Framework.Common.CommonFunc;
using Sinboda.Framework.Common.IOTOperate;
using Sinboda.Framework.Common.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sinboda.Framework.MainWindow.Blue.Views
{
    /// <summary>
    /// MessageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MessageWindow : Window
    {
        public static readonly string targetTitle = "Diruiyun.Iot.Service.Client";
        public MessageWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //(PresentationSource.FromVisual(this) as HwndSource).AddHook(new HwndSourceHook(this.WndProc));
            HwndSource hwndSource = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            if (hwndSource != null)
            {
                hwndSource.AddHook(new HwndSourceHook(WndProc));
            }
            this.Opacity = 0;
            this.Hide();
        }
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            try
            {
                if (msg == MessageHelper.WM_COPYDATA)
                {
                    MsgDataStruct cds = (MsgDataStruct)Marshal.PtrToStructure(lParam, typeof(MsgDataStruct));
                    dynamic dy = JsonConvert.DeserializeObject(cds.lpData);
                    if (dy.method == "getDeviceOptions")
                    {
                        string result = IOTOperateManager.OnDeviceEvent();
                        if (!string.IsNullOrEmpty(result))
                        {
                            MessageHelper.SendMessage(targetTitle, result);
                        }
                    }
                    else if (dy.method == "execute")
                    {
                        if (dy.@params != null && dy.@params.@type != null)
                        {
                            string type = (string)dy.@params.@type;
                            string value = (string)dy.@params.@value;
                            if (type == "sql")
                            {
                                IOTOperateManager.OnExecuteSQLEvent(value, targetTitle);
                            }
                            else
                            {
                                IOTOperateManager.OnExecuteEvent(type, value, targetTitle);
                            }
                        }

                    }
                    else if (dy.method == "isBusy")
                    {
                        string result = IOTOperateManager.OnIsBusyEvent();
                        if (!string.IsNullOrEmpty(result))
                        {
                            MessageHelper.SendMessage(targetTitle, result);
                        }
                    }
                    else if (dy.method == "register")
                    {
                        string code = dy.@code;
                        string reegCode = string.Empty;
                        if (dy.@data != null)
                            reegCode = dy.@data.@RegisterCode;


                        LogHelper.logSoftWare.Debug($"[register reply] : {cds.lpData}");
                        IOTOperateManager.IotRegisterCompleted(code, reegCode);
                    }
                    else
                    {
                        // 远程返回的操作结果 sunch 20220622
                        LogHelper.logSoftWare.Debug($"[AsyncSendInfo reply] : {cds.lpData}");

                        string method = dy.@method;
                        string code = dy.@code;
                        string data = dy.@data.@Data;

                        IOTOperateManager.IotAsyncSendInfoCompleted(method, code, data);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Debug($"[远程 WndProc ] 异常：{ex.Message}");
            }
            return hwnd;
        }
    }
}
