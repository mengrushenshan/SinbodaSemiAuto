using Sinboda.Framework.Common.DBOperateHelper;
using Sinboda.Framework.Common.FileOperateHelper;
using Sinboda.SemiAuto.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sinboda.SemiAuto.Core.Models
{
    public class GlobalData
    {
        /// <summary>
        /// ximc电机最大有效行程
        /// </summary>
        public static int MaxXimcDistance = 20000;

        /// <summary>
        /// ximc电机最小有效行程
        /// </summary>
        public static int MinXimcDistance = -20000;

        /// <summary>
        /// 视频保存路径
        /// </summary>
        public readonly static string DirectoryVideo = $"{System.IO.Directory.GetCurrentDirectory()}\\Video";

        /// <summary>
        /// 图片保存路径
        /// </summary>
        public readonly static string DirectoryPic = $"{System.IO.Directory.GetCurrentDirectory()}\\Pic";

        /// <summary>
        /// xml数据路径
        /// </summary>
        public static string DicXml => $"{System.IO.Directory.GetCurrentDirectory()}\\Xml";

        /// <summary>
        /// 机械臂数据保存地址
        /// </summary>
        public readonly static string FileNameConfigData = $"Config.xml";

        /// <summary>
        /// 电机参数
        /// </summary>
        public static XimcArmsData XimcArmsData { get; set; } = new XimcArmsData();

        /// <summary>
        /// 曝光时间 us
        /// </summary>
        public static uint ExposureTime { get; set; } = 30;

        /// <summary>
        /// 获取帧数量
        /// </summary>
        public static int FrameNum { get; set; } = 1;

        /// <summary>
        /// 超时 秒
        /// </summary>
        public const int TimeOut = 100;

        /// <summary>
        /// 孔板行数量
        /// </summary>
        public const int PlateRow = 3;

        /// <summary>
        /// 孔板列数量
        /// </summary>
        public const int PlateCol = 10;

        /// <summary>
        /// 接收器间隔 毫秒
        /// </summary>
        public const int CommReceiveInterval = 100;

        /// <summary>
        /// ximc电机慢速 steps/s
        /// </summary>
        public const uint XimcSlowSpeed = 300;

        /// <summary>
        /// ximc电机慢速 加速度 steps/s^2
        /// </summary>
        public const uint XimcSlowAccel = 300;

        /// <summary>
        /// ximc电机快速 steps/s
        /// </summary>
        public const uint XimcFastSpeed = 3000;

        /// <summary>
        /// ximc电机快速 加速度 steps/s^2
        /// </summary>
        public const uint XimcFastAccel = 1000;

        /// <summary>
        /// ip地址
        /// </summary>
        public static string IpAddress { get; set; } = "192.168.1.30";

        /// <summary>
        /// ip端口
        /// </summary>
        public static int Port { get; set; } = 2023;

        //视频宽，高 帧数
        public const int VideoWidth = 2048;
        public const int VideoHeight = 2048;
        public const int VideoFPS = 20;

        //载玻片可视距离 微米
        public const int SlidesWidth = 25 * 1000;
        public const int SlidesHeight = 25 * 1000;

        /// <summary>
        /// 帧编号
        /// </summary>
        public static int FrameID
        {
            get
            {
                if (_frameID >= UInt16.MaxValue)
                    _frameID = 0;
                return _frameID++;
            }
        }
        private static int _frameID = 0;

        //消息通知的令牌
        public const string TokenCamera = "Camera";
        public const string TokenAlarmData = "AlarmData";
        //¼üÊóÊÂ¼þ
        public const string WinMouseWheelEvent = "MouseWheelEvent";
        public const string WinKeyBoardEvent = "KeyBoardEvent";
        public const string WinMouseDown = "MouseDown";
        public const string WinMouseMove = "MouseMove";
        public const string WinMouseUp = "MouseUp";
        //tcpÖ÷ÍÆÏûÏ¢ÁîÅÆ
        public const string TokenTcpMsg = "TokenTcpMag";

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            Config config = new Config();
            config = XmlHelper.GetXmlData<Config>(FileNameConfigData);
            //按照使用编号 重新排序
            config.XimcArmsData.XimcArms.Sort((x, y) => x.CtrlName.CompareTo(y.CtrlName));
            GlobalData.XimcArmsData = config.XimcArmsData;
            GlobalData.ExposureTime = config.ExposureTime;
            GlobalData.FrameNum = config.FrameNum;
            GlobalData.IpAddress = config.IpAddress;
            GlobalData.Port = config.Port;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="name"></param>
        public static void Save()
        {
            Config config = new Config();
            config.XimcArmsData = GlobalData.XimcArmsData;
            config.ExposureTime = GlobalData.ExposureTime;
            config.FrameNum = GlobalData.FrameNum;
            config.IpAddress = GlobalData.IpAddress;
            config.Port = GlobalData.Port;
            XmlHelper.SaveXml(FileNameConfigData, config);
        }
    }

    [Serializable]
    public class Config
    {
        [XmlElement("MotorArmsData")]
        public XimcArmsData XimcArmsData { get; set; } = new XimcArmsData();

        /// <summary>
        /// 曝光时间 us
        /// </summary>
        [XmlElement("ExposureTime")]
        public uint ExposureTime { get; set; } = 30;

        /// <summary>
        /// 刷新率 帧/秒
        /// </summary>
        [XmlElement("FrameNum")]
        public int FrameNum { get; set; } = 1;

        /// <summary>
        /// ip地址
        /// </summary>
        [XmlElement("IpAddress")]
        public string IpAddress { get; set; } = "192.168.1.30";

        /// <summary>
        /// ip端口
        /// </summary>
        [XmlElement("Port")]
        public int Port { get; set; } = 2023;

    }
}
