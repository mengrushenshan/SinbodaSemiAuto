using Sinboda.Framework.Common.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sinboda.Framework.Common.ResourceExtensions
{
    /// <summary>
    /// 多语言界面属性绑定实现类
    /// </summary>
    [MarkupExtensionReturnType(typeof(BindingExpression))]
    public class StringResourceExtension : MarkupExtension, INotifyPropertyChanged
    {
        /// <summary>
        /// 语言
        /// </summary>
        public static string[] LanguageArray { get; set; }

        /// <summary>
        /// 获取系统词条
        /// </summary>
        /// <param name="lid">语言ID</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="args">词条参数</param>
        /// <returns></returns>
        public static string GetLanguage(int lid, string defaultValue = "", params object[] args)
        {
            try
            {
                string lvalue = string.Empty;
                if (lid < StringResourceExtension.LanguageArray.Length)
                {
                    lvalue = StringResourceExtension.LanguageArray[lid];
                }

                string value = string.IsNullOrEmpty(lvalue) ? defaultValue : lvalue;
                return args == null ? value : string.Format(value, args);
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Debug($"获取系统词条异常：语言编号={lid}", ex);
                return defaultValue;
            }
        }

        /// <summary>
        /// 资源的名称，与资源文件StringResource.resx对应
        /// </summary>
        [ConstructorArgument("key")]
        public int Key
        {
            get;
            set;
        }
        string _DefaultValue;
        /// <summary>
        /// 默认值，为了使在设计器的情况时把默认值绑到设计器
        /// </summary>
        public string DefaultValue
        {
            get
            {
                return _DefaultValue;
            }
            set
            {
                _DefaultValue = value;
            }
        }
        string _Value;
        /// <summary>
        /// 资源的具体内容，通过资源名称也就是上面的Key找到对应内容
        /// </summary>
        public string Value
        {
            get
            {
                if (DesignMode.IsInDesignMode) return _DefaultValue;

                if (Key != 0)
                {
                    string strResault = null;
                    try
                    {
                        strResault = LanguageArray[Key];
                    }
                    catch (Exception e)
                    {
                        strResault = _DefaultValue;
                        LogHelper.logSoftWare.Debug("语言索引异常，key值为：" + Key, e);
                    }
                    finally
                    {
                        if (strResault == null)
                        {
                            strResault = _DefaultValue;
                            LogHelper.logSoftWare.Debug("语言键值不存在，key值为：" + Key);
                        }
                    }
                    return strResault;
                }
                else
                {
                    return _DefaultValue;
                }
            }
            set
            {
                _Value = value;
            }
        }

        /// <summary>
        /// 多语言绑定接口
        /// </summary>
        /// <param name="key"></param>
        public StringResourceExtension(int key)
            : this()
        {
            Key = key;
            GlobalClass.LanguageChangeEvent += new EventHandler<EventArgs>(Language_Event);
        }

        /// <summary>
        /// 多语言绑定接口
        /// </summary>
        /// <param name="key"></param>
        /// <param name="DefaultValue"></param>
        public StringResourceExtension(int key, string DefaultValue)
            : this()
        {
            Key = key;
            _DefaultValue = DefaultValue;
            GlobalClass.LanguageChangeEvent += new EventHandler<EventArgs>(Language_Event);
        }

        /// <summary>
        /// 多语言绑定接口
        /// </summary>
        public StringResourceExtension()
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
        /// 语言改变通知事件，当程序初始化的时候会绑定到全局的GlobalClass.LanguageChangeEvent事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Language_Event(object sender, EventArgs e)
        {
            //通知Value值已经改变，需重新获取
            NotifyValueChanged();
        }
    }

    /// <summary>
    /// 多语言切换通知类
    /// </summary>
    public static class GlobalClass
    {

        /// <summary>
        /// 语言改变通知事件
        /// </summary>
        public static EventHandler<EventArgs> LanguageChangeEvent;

        static GlobalClass()
        {
        }

        /// <summary>
        /// 多语言切换
        /// </summary>
        public static void ChangeLanguage()
        {
            if (LanguageChangeEvent != null)
            {
                LanguageChangeEvent(null, null);
            }
        }
    }
    /// <summary>
    /// 设计器帮助类
    /// </summary>
    public static class DesignMode
    {
        /// <summary>
        /// 是否为设计模式
        /// </summary>
        public static bool IsInDesignMode
        {
            get
            {
                DependencyPropertyDescriptor dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement));
                bool? isInDesignMode = new bool?((bool)dependencyPropertyDescriptor.Metadata.DefaultValue);
                return isInDesignMode.Value;
            }
        }
    }
}
