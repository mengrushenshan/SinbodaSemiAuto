using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Resources
{
    public static class MessageToken
    {
        //图像接收通知
        public static string TokenCamera = "Camera";
        public static string TokenKeyDown = "KeyDown";
        public static string TokenCameraBuffer = "CameraBuffer";

        //图像采集完成通知
        public static string AcquiringImageComplete = "AcquiringImage";
        /// <summary>
        /// 仪器检查状态
        /// </summary>
        public static string EXAM_STATUS = "exam_status";
        /// <summary>
        /// 维护刷新界面
        /// </summary>
        public static string MAINTANCE_REFRESH = "maintance_refresh";

        /// <summary>
        /// 维护刷新界面
        /// </summary>
        public static string MACHINE_REFRESH = "machine_refresh";

    }
}
