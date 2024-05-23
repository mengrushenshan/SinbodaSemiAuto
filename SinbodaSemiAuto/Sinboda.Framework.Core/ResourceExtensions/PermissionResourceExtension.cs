using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;

namespace Sinboda.Framework.Core.ResourceExtensions
{
    /// <summary>
    /// 权限界面属性绑定实现类
    /// </summary>
    [MarkupExtensionReturnType(typeof(BindingExpression))]
    public class PermissionResourceExtension : MarkupExtension, INotifyPropertyChanged
    {
        /// <summary>
        /// 资源的名称，与资源文件StringResource.resx对应
        /// </summary>
        [ConstructorArgument("key")]
        public string Key
        {
            get;
            set;
        }
        string _Value;
        /// <summary>
        /// 资源的具体内容，通过资源名称也就是上面的Key找到对应内容
        /// </summary>
        public string Value
        {
            get
            {
                if (!string.IsNullOrEmpty(Key))
                {
                    string strResault = null;
                    try
                    {
                        strResault = SystemResources.Instance.CurrentPermissionList[Key] == true ? "Visible" : "Collapsed";
                    }
                    catch (Exception e)
                    {
                        LogHelper.logSoftWare.Debug("权限索引异常，key值为：" + Key, e);
                    }
                    if (strResault == null)
                    {
                        strResault = "Visible";
                        LogHelper.logSoftWare.Debug("权限键值不存在，key值为：" + Key);
                    }
                    return strResault;
                }
                else
                {
                    return "Visible";
                }
            }
            set
            {
                _Value = value;
            }
        }

        /// <summary>
        /// 权限绑定接口
        /// </summary>
        /// <param name="key"></param>
        public PermissionResourceExtension(string key)
            : this()
        {
            Key = key;
            GlobalClass.PermissionChangeEvent += new EventHandler<EventArgs>(Permission_Event);
        }

        /// <summary>
        /// 权限绑定接口
        /// </summary>
        public PermissionResourceExtension()
        {

        }

        /// <summary>
        /// 每一标记扩展实现的 ProvideValue 方法能在可提供上下文的运行时使用 IServiceProvider。然后会查询此 IServiceProvider 以获取传递信息的特定服务
        /// 当 XAML 处理器在处理一个类型节点和成员值，且该成员值是标记扩展时，它将调用该标记扩展的 ProvideValue 方法并将结果写入到对象关系图或序列化流,XAML 对象编写器将服务环境通过 serviceProvider 参数传递到每个此类实现。
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            Setter setter = target.TargetObject as Setter;
            if (setter != null)
            {
                return new Binding("Value") { Source = this, Mode = BindingMode.OneWay };
            }
            else
            {
                Binding binding = new Binding("Value") { Source = this, Mode = BindingMode.OneWay };
                return binding.ProvideValue(serviceProvider);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        static readonly PropertyChangedEventArgs valueChangedEventArgs = new PropertyChangedEventArgs("Value");
        /// <summary>
        /// 
        /// </summary>
        protected void NotifyValueChanged()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, valueChangedEventArgs);
        }

        /// <summary>
        /// 权限改变通知事件，当程序初始化的时候会绑定到全局的GlobalClass.PermissionChangeEvent事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Permission_Event(object sender, EventArgs e)
        {
            //通知Value值已经改变，需重新获取
            NotifyValueChanged();
        }
    }

    /// <summary>
    /// 权限切换通知类
    /// </summary>
    public static class GlobalClass
    {

        /// <summary>
        /// 权限改变通知事件
        /// </summary>
        public static EventHandler<EventArgs> PermissionChangeEvent;

        static GlobalClass()
        {
        }

        /// <summary>
        /// 权限切换
        /// </summary>
        public static void ChangeLanguage()
        {
            if (PermissionChangeEvent != null)
            {
                PermissionChangeEvent(null, null);
            }
        }
    }
}
