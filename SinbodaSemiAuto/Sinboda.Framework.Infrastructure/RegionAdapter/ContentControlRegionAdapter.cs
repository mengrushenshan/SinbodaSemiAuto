using Sinboda.Framework.Common.ResourceExtensions;
using Sinboda.Framework.Infrastructure.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Sinboda.Framework.Infrastructure.RegionAdapter
{
    /// <summary>
    /// ContentControl 类型的区域初始化实现
    /// </summary>
    public class ContentControlRegionAdapter : IRegionAdapter
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="regionTarget"></param>
        /// <param name="region"></param>
        public void Initialize(object regionTarget, object region)
        {
            ContentControl contentControl = GetCastedObject(regionTarget);
            contentControl.Content = region;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="regionTarget"></param>
        /// <returns></returns>
        private static ContentControl GetCastedObject(object regionTarget)
        {
            if (regionTarget == null)
            {
                throw new ArgumentNullException("regionTarget");
            }
            ContentControl t = regionTarget as ContentControl;
            if (t == null)
            {
                throw new InvalidOperationException(StringResourceExtension.GetLanguage(154, "内容类型转换失败", typeof(ContentControl).Name)); //TODO 翻译
            }
            return t;
        }
    }
}
