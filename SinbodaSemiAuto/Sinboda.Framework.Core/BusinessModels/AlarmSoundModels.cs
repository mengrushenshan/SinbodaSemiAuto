using Sinboda.Framework.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.BusinessModels
{
    public class AlarmSoundModels
    {
        /// <summary>
        /// 报警级别
        /// </summary>
        public AlarmLevelEnum CodeLevel { get; set; }

        /// <summary>
        /// 提示音是否开启
        /// </summary>
        public bool HaveSound { get; set; }

        /// <summary>
        /// 提示音自定义文件
        /// </summary>
        public string SoundFile { get; set; }

        /// <summary>
        /// 提示音对应报警码
        /// </summary>
        public List<string> AlarmCodeList { get; set; }
    }
}
