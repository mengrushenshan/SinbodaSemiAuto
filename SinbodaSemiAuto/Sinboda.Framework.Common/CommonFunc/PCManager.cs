using Sinboda.Framework.Common;
using Sinboda.Framework.Common.FileOperateHelper;
using Sinboda.Framework.Common.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sinboda.Framework.Common.CommonFunc
{

    [XmlRoot(ElementName = "SystemWarningSetting")]
    public class SystemWarningSetting
    {
        public SystemWarningSetting()
        {
            BootTime = "None";
        }

        [XmlAttribute]
        public string BootTime { get; set; }

        [XmlArray("DriveFreeSpaces")]
        [XmlArrayItem("DriveFreeSpace", typeof(DriveFreeSpace))]
        public List<DriveFreeSpace> DriveFreeSpaces { get; set; }
    }

    [XmlRoot("DriveFreeSpace")]
    public class DriveFreeSpace
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string AlarmVaue { get; set; }

        [XmlAttribute]
        public string ErrorValue { get; set; }
    }

    public enum DirveAlrLevel
    {
        Alarm,
        Error
    }

    public class DriveResult
    {
        public string Name { get; set; }

        /// <summary>
        /// 设置空间
        /// </summary>
        public double FreeSpace { get; set; }

        /// <summary>
        /// 实际剩余空间
        /// </summary>
        public double ActualFreeSpace { get; set; }

        public DirveAlrLevel Level { get; set; }
    }


    public sealed class PCManager
    {
        public static SystemWarningSetting PCSetting { get; private set; }

        static PCManager()
        {
            var setting = ReadSystemWarningSetting();
            PCSetting = setting ?? new SystemWarningSetting();
        }


        /// <summary>
        /// 检查开机时间
        /// </summary>
        /// <returns></returns>
        public static bool CheckBootTime()
        {
            double bt = 0;
            if (double.TryParse(PCSetting.BootTime, out bt))
            {
                var tick = DateTime.Now.AddMilliseconds(1 - Environment.TickCount);
                TimeSpan ts = DateTime.Now.Subtract(tick);

                return ts.TotalHours > bt;
            }
            return true;
        }


        /// <summary>
        /// 检查磁盘空间
        /// </summary>
        /// <returns></returns>
        public static List<DriveResult> CheckDriveSpace()
        {
            var result = new List<DriveResult>();
            if (PCSetting.DriveFreeSpaces.Count <= 0)
                return result;

            foreach (var drive in DriveInfo.GetDrives().Where(o => o.DriveType == DriveType.Fixed))
            {
                var tem = PCSetting.DriveFreeSpaces.FirstOrDefault(o => o.Name == drive.Name);
                if (tem == null)
                    continue;

                var alarmVaue = ConvertDirveSize(tem.AlarmVaue);
                var errorValue = ConvertDirveSize(tem.ErrorValue);
                var totalFreeSpace = drive.TotalFreeSpace;

                if(errorValue.HasValue && errorValue.Value > totalFreeSpace)
                    result.Add(new DriveResult { Name = tem.Name, Level = DirveAlrLevel.Error, ActualFreeSpace = totalFreeSpace, FreeSpace = errorValue.Value });

                if (alarmVaue.HasValue && alarmVaue.Value > totalFreeSpace)
                    result.Add(new DriveResult { Name = tem.Name, Level = DirveAlrLevel.Alarm, ActualFreeSpace = totalFreeSpace, FreeSpace = alarmVaue.Value });
            }

            return result;
        }

        /// <summary>
        /// 单位 MB
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static double? ConvertDirveSize(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            double tem = 0;
            if (double.TryParse(value, out tem))
                return tem * 1024 * 1024;

            return null;
        }


        private static SystemWarningSetting ReadSystemWarningSetting()
        {
            try
            {
                string path = MapPath.XmlPath + @"SystemWarningSetting.xml";
                LogHelper.logSoftWare.Debug("GetSystemWarningSetting ");
                SystemWarningSetting model = null;
                if (File.Exists(path))
                {
                    FileHelper _helper = new FileHelper();
                    model = _helper.ReadXML<SystemWarningSetting>(path);
                }
                return model;
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("GetSystemWarningSetting ", e);
                return null;
            }
        }
    }
}
