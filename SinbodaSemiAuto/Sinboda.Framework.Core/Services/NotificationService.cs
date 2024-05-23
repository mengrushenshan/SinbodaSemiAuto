using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Sinboda.Framework.Control.Controls;
using System.Xml;
using Sinboda.Framework.Common;

namespace Sinboda.Framework.Core.Services
{
    /// <summary>
    /// 按钮自动关闭类型
    /// </summary>
    public enum BtnAutoCloseTye
    {
        Yes,
        No,
        Ok,
        Cancle
    };

    /// <summary>
    /// 用户信息通知服务
    /// </summary>
    public sealed class NotificationService : TBaseSingleton<NotificationService>
    {

        /// <summary>
        /// 显示提示信息
        /// </summary>
        /// <param name="messageBoxText">内容</param>
        /// <param name="autoclose">是否启用倒计时自动关闭</param>
        /// <returns></returns>
        public MessageBoxResult ShowMessage(string messageBoxText, bool autoclose = false)
        {
            return ShowMessage(GetCaption(SinMessageBoxImage.None), messageBoxText, MessageBoxButton.OK, SinMessageBoxImage.None, autoclose);
        }

        /// <summary>
        /// 显示提示信息
        /// </summary>
        /// <param name="messageBoxText">内容</param>
        /// <param name="button">按钮</param>
        /// <param name="autoclose">是否启用倒计时自动关闭</param>
        /// <returns></returns>
        public MessageBoxResult ShowMessage(string messageBoxText, MessageBoxButton button, bool autoclose = false)
        {
            return ShowMessage(GetCaption(SinMessageBoxImage.None), messageBoxText, button, SinMessageBoxImage.None, autoclose);
        }

        /// <summary>
        /// 显示询问提示
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="autoclose">是否启用倒计时自动关闭</param>
        /// <returns></returns>
        public MessageBoxResult ShowQuestion(string messageBoxText, bool autoclose = false)
        {
            return ShowMessage(GetCaption(SinMessageBoxImage.Question), messageBoxText, MessageBoxButton.YesNo, SinMessageBoxImage.Question, autoclose);
        }

        public MessageBoxResult ShowQuestionAutoClose(string messageBoxText, int btnAutoCloseTime = 60)
        {
            return ShowMessage(GetCaption(SinMessageBoxImage.Question), messageBoxText, MessageBoxButton.YesNo, SinMessageBoxImage.Question, false, true, btnAutoCloseTime);
        }

        /// <summary>
        /// 显示错误提示
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="autoclose">是否启用倒计时自动关闭</param>
        /// <returns></returns>
        public MessageBoxResult ShowError(string messageBoxText, bool autoclose = false)
        {
            return ShowMessage(GetCaption(SinMessageBoxImage.Error), messageBoxText, MessageBoxButton.OK, SinMessageBoxImage.Error, autoclose);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="autoclose"></param>
        /// <returns></returns>
        public MessageBoxResult ShowCompleted(string messageBoxText, bool autoclose = true)
        {
            return ShowMessage(GetCaption(SinMessageBoxImage.Completed), messageBoxText, MessageBoxButton.OK, SinMessageBoxImage.Completed, autoclose);
        }

        /// <summary>
        /// 显示提示信息
        /// </summary>
        /// <param name="messageBoxText">内容</param>
        /// <param name="button">按钮</param>
        /// <param name="icon">图标（DrMessageBoxImage.Completed时2s自动关闭）</param>
        /// <returns></returns>
        public MessageBoxResult ShowMessage(string messageBoxText, MessageBoxButton button, SinMessageBoxImage icon)
        {
            return ShowMessage(GetCaption(icon), messageBoxText, button, icon, icon == SinMessageBoxImage.Completed);
        }

        /// <summary>
        /// 显示提示信息
        /// </summary>
        /// <param name="messageBoxText">内容</param>
        /// <param name="button">按钮</param>
        /// <param name="icon">图标</param>
        /// <param name="autoclose">是否启用倒计时自动关闭</param>
        /// <returns></returns>
        public MessageBoxResult ShowMessage(string messageBoxText, MessageBoxButton button, SinMessageBoxImage icon, bool autoclose)
        {
            return ShowMessage(GetCaption(icon), messageBoxText, button, icon, autoclose);
        }

        /// <summary>
        /// 显示提示信息
        /// </summary>
        /// <param name="caption">标题</param>
        /// <param name="messageBoxText">内容</param>
        /// <param name="button">按钮</param>
        /// <param name="icon">图标</param>
        /// <param name="autoclose">是否启用倒计时自动关闭</param>
        /// <returns></returns>
        public MessageBoxResult ShowMessage(string caption, string messageBoxText, MessageBoxButton button, SinMessageBoxImage icon, bool autoclose = false, bool btnAutoClose = false, int btnAutoCloseTime = 60)
        {
            object result = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                int autoclosetime = 0;
                if (autoclose)
                {
                    XmlDocument xd = new XmlDocument();
                    xd.Load(MapPath.XmlPath + "MESSAGE_CONFIG.xml");
                    XmlNodeList NodeList1 = xd.DocumentElement.ChildNodes;
                    autoclosetime = int.Parse(NodeList1[0].InnerText.ToString());
                }
                SinMessageBox box = new SinMessageBox(caption, messageBoxText, icon, autoclosetime, SystemResources.Instance.LanguageArray[6577]);//自动关闭
                box.AddButtons(GetButtons(box, button, null, btnAutoClose, btnAutoCloseTime));
                box.ShowDialog();
                result = box.Result;
            });

            // 为 Null 暂时返回‘取消’
            if (result == null)
                return MessageBoxResult.Cancel;

            LogHelper.logSoftWare.Info("caption：" + caption + "；  msgtxt：" + messageBoxText + "；  result：" + result.ToString());
            return (MessageBoxResult)result;
        }

        /// <summary>
        /// 显示信息，自动一定时间不点击确定，自动执行确认动作
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="messageBoxText"></param>
        /// <param name="button"></param>
        /// <param name="icon"></param>
        /// <param name="btnAutoYesClose"></param>
        /// <param name="btnAutoYesCloseTime"></param>
        /// <returns></returns>
        public MessageBoxResult ShowMessage(string caption, string messageBoxText, MessageBoxButton button, SinMessageBoxImage icon, BtnAutoCloseTye AutoCloseType, int btnAutoCloseTime = 60)
        {
            object result = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                int autoclosetime = btnAutoCloseTime;

                SinMessageBox box = new SinMessageBox(caption, messageBoxText, icon, 0, SystemResources.Instance.LanguageArray[6577]);//自动关闭

                box.AddButtons(GetButtons(box, button, AutoCloseType, autoclosetime));
                box.ShowDialog();
                result = box.Result;
            });

            // 为 Null 暂时返回‘取消’
            if (result == null)
                return MessageBoxResult.Cancel;

            LogHelper.logSoftWare.Info("caption：" + caption + "；  msgtxt：" + messageBoxText + "；  result：" + result.ToString());
            return (MessageBoxResult)result;
        }

        /// <summary>
        /// 显示提示信息 (只能在UI线程新建Button)
        /// </summary>
        /// <param name="caption">标题</param>
        /// <param name="messageBoxText">内容</param>
        /// <param name="buttons">自定义按钮</param>
        /// <param name="button">按钮</param>
        /// <param name="icon">图标</param>
        /// <param name="isAdd">是否增加按钮，否的化只是修改原来按钮显示文言</param>
        /// <returns></returns>
        public MessageBoxResult ShowMessage(string caption, string messageBoxText, MessageBoxButton button, SinMessageBoxImage icon, bool isAdd, params Button[] buttons)
        {
            object result = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                SinMessageBox box = new SinMessageBox(caption, messageBoxText, icon);
                if (isAdd)
                {
                    box.AddButtons(buttons);
                }

                List<string> contentList = new List<string>();
                for (int i = 0; i < buttons.Length; i++)
                {
                    contentList.Add(buttons[i].Content.ToString());
                }

                box.AddButtons(GetButtons(box, button, contentList));
                box.ShowDialog();
                result = box.Result;
            });

            // 为 Null 暂时返回‘取消’
            if (result == null)
            {
                return MessageBoxResult.Cancel;
            }

            LogHelper.logSoftWare.Info("caption：" + caption + "； msgtxt：" + messageBoxText + "； result：" + result.ToString());
            return (MessageBoxResult)result;
        }
        /// <summary>
        /// 显示提示信息
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="messageBoxText"></param>
        /// <param name="button"></param>
        /// <param name="icon"></param>
        /// <param name="buttonContents">各个Button显示名称</param>
        /// <returns></returns>
        public MessageBoxResult ShowMessage(string caption, string messageBoxText, MessageBoxButton button, SinMessageBoxImage icon, params string[] buttonContents)
        {
            object result = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                SinMessageBox box = new SinMessageBox(caption, messageBoxText, icon);

                List<string> buttonContentList = new List<string>();
                for (int i = 0; i < buttonContents.Length; i++)
                {
                    buttonContentList.Add(buttonContents[i]);
                }

                box.AddButtons(GetButtons(box, button, buttonContentList));
                box.ShowDialog();
                result = box.Result;
            });

            // 为 Null 暂时返回‘取消’
            if (result == null)
            {
                return MessageBoxResult.Cancel;
            }

            LogHelper.logSoftWare.Info("caption：" + caption + "； msgtxt：" + messageBoxText + "； result：" + result.ToString());
            return (MessageBoxResult)result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        private string GetCaption(SinMessageBoxImage icon)
        {
            // TODO：GetCaption 未翻译
            switch (icon)
            {
                case SinMessageBoxImage.None:
                    return string.Empty;
                case SinMessageBoxImage.Completed:
                    return SystemResources.Instance.LanguageArray[283]; //"完成";
                case SinMessageBoxImage.Error:
                    return SystemResources.Instance.LanguageArray[816]; //"错误";
                case SinMessageBoxImage.Information:
                    return SystemResources.Instance.LanguageArray[29]; //"提示";
                case SinMessageBoxImage.Question:
                    return SystemResources.Instance.LanguageArray[2197]; //"询问";
                case SinMessageBoxImage.Stop:
                    return SystemResources.Instance.LanguageArray[814]; //"停止";
                case SinMessageBoxImage.Warning:
                    return SystemResources.Instance.LanguageArray[815]; //"警告";
                default:
                    return string.Empty;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="box"></param>
        /// <param name="button"></param>
        /// <param name="firstText">第一个显示内容</param>
        /// <param name="secondText">第二个显示内容</param>
        /// <returns></returns>
        private IEnumerable<Button> GetButtons(SinMessageBox box, MessageBoxButton button, List<string> contentList, bool btnAutoClose = false, int btnAutoCloseTime = 60)
        {
            string strFirstText = string.Empty;
            string strSecondText = string.Empty;
            string strThirdText = string.Empty;

            if (null != contentList)
            {
                strFirstText = (0 < contentList.Count ? contentList[0] : strFirstText);
                strSecondText = (1 < contentList.Count ? contentList[1] : strSecondText);
                strThirdText = (2 < contentList.Count ? contentList[2] : strThirdText);
            }

            // TODO：GetButtons 未翻译
            if (button == MessageBoxButton.OK)
            {
                if (string.IsNullOrEmpty(strFirstText))
                {
                    yield return box.CreateButton(SystemResources.Instance.LanguageArray[142], true, false, MessageBoxResult.OK, btnAutoClose, btnAutoCloseTime);//确定
                }
                else
                {
                    yield return box.CreateButton(strFirstText, true, false, MessageBoxResult.OK, btnAutoClose, btnAutoCloseTime);//确定
                }
            }
            else if (button == MessageBoxButton.OKCancel)
            {
                if (string.IsNullOrEmpty(strFirstText))
                {
                    yield return box.CreateButton(SystemResources.Instance.LanguageArray[142], true, false, MessageBoxResult.OK);//确定
                }
                else
                {
                    yield return box.CreateButton(strFirstText, true, false, MessageBoxResult.OK);//确定
                }

                if (string.IsNullOrEmpty(strSecondText))
                {
                    yield return box.CreateButton(SystemResources.Instance.LanguageArray[143], false, true, MessageBoxResult.Cancel);//取消
                }
                else
                {
                    yield return box.CreateButton(strSecondText, false, true, MessageBoxResult.Cancel);//取消
                }
            }
            else if (button == MessageBoxButton.YesNo)
            {
                yield return box.CreateButton(SystemResources.Instance.LanguageArray[5], true, false, MessageBoxResult.Yes);//是
                yield return box.CreateButton(SystemResources.Instance.LanguageArray[6], false, true, MessageBoxResult.No, btnAutoClose, btnAutoCloseTime);//否
            }
            else if (button == MessageBoxButton.YesNoCancel)
            {
                if (string.IsNullOrEmpty(strFirstText))
                {
                    yield return box.CreateButton(SystemResources.Instance.LanguageArray[5], true, false, MessageBoxResult.Yes);//是
                }
                else
                {
                    yield return box.CreateButton(strFirstText, true, false, MessageBoxResult.Yes);//是
                }

                if (string.IsNullOrEmpty(strSecondText))
                {
                    yield return box.CreateButton(SystemResources.Instance.LanguageArray[6], false, true, MessageBoxResult.No);//否
                }
                else
                {
                    yield return box.CreateButton(strSecondText, false, true, MessageBoxResult.No);//否
                }

                if (string.IsNullOrEmpty(strThirdText))
                {
                    yield return box.CreateButton(SystemResources.Instance.LanguageArray[143], false, true, MessageBoxResult.Cancel);//取消
                }
                else
                {
                    yield return box.CreateButton(strThirdText, false, true, MessageBoxResult.Cancel);//取消
                }
            }
        }

        private IEnumerable<Button> GetButtons(SinMessageBox box, MessageBoxButton button, BtnAutoCloseTye autoCloseType, int btnAutoCloseTime)
        {
            if (button == MessageBoxButton.YesNo)
            {
                if (autoCloseType == BtnAutoCloseTye.Yes)
                {
                    yield return box.CreateButton(SystemResources.Instance.LanguageArray[5], true, false, MessageBoxResult.Yes, true, btnAutoCloseTime);//是
                    yield return box.CreateButton(SystemResources.Instance.LanguageArray[6], false, true, MessageBoxResult.No);//否
                }
                else if (autoCloseType == BtnAutoCloseTye.No)
                {
                    yield return box.CreateButton(SystemResources.Instance.LanguageArray[5], true, false, MessageBoxResult.Yes);//是
                    yield return box.CreateButton(SystemResources.Instance.LanguageArray[6], false, true, MessageBoxResult.No, true, btnAutoCloseTime);//否
                }
                else
                {
                    yield return box.CreateButton(SystemResources.Instance.LanguageArray[5], true, false, MessageBoxResult.Yes);//是
                    yield return box.CreateButton(SystemResources.Instance.LanguageArray[6], false, true, MessageBoxResult.No);//否
                }
            }
            else if (button == MessageBoxButton.OKCancel)
            {
                if (autoCloseType == BtnAutoCloseTye.Ok)
                {
                    yield return box.CreateButton(SystemResources.Instance.LanguageArray[142], true, false, MessageBoxResult.Yes, true, btnAutoCloseTime);//是
                    yield return box.CreateButton(SystemResources.Instance.LanguageArray[143], false, true, MessageBoxResult.No);//否
                }
                else if (autoCloseType == BtnAutoCloseTye.Cancle)
                {
                    yield return box.CreateButton(SystemResources.Instance.LanguageArray[142], true, false, MessageBoxResult.Yes);//是
                    yield return box.CreateButton(SystemResources.Instance.LanguageArray[143], false, true, MessageBoxResult.No, true, btnAutoCloseTime);//否
                }
                else
                {
                    yield return box.CreateButton(SystemResources.Instance.LanguageArray[142], true, false, MessageBoxResult.Yes);//是
                    yield return box.CreateButton(SystemResources.Instance.LanguageArray[143], false, true, MessageBoxResult.No);//否
                }
            }
        }
    }
}
