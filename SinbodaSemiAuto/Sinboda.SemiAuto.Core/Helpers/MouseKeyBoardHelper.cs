using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Helpers
{
    public class MouseKeyBoardHelper
    {
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int nVirtKey);

        /// <summary>
        /// 鼠标左键是否按下
        /// </summary>
        /// <returns></returns>
        public static bool IsLeftBtnDown()
        {
            int code = GetAsyncKeyState(0x01) & 0x8000;
            return code != 0;
        }

        /// <summary>
        /// ctrl是否按下 左右都能触发
        /// </summary>
        /// <returns></returns>
        public static bool IsCtrlDown()
        {
            int code = GetAsyncKeyState(0x11) & 0x8000;
            return code != 0;
        }

        /// <summary>
        /// 左箭头键是否按下
        /// </summary>
        /// <returns></returns>
        public static bool Is_VK_LEFT_Down()
        {
            int code = GetAsyncKeyState(0x25) & 0x8000;
            return code != 0;
        }

        /// <summary>
        /// 上箭头键是否按下
        /// </summary>
        /// <returns></returns>
        public static bool Is_VK_UP_Down()
        {
            int code = GetAsyncKeyState(0x26) & 0x8000;
            return code != 0;
        }

        /// <summary>
        /// 右箭头键是否按下
        /// </summary>
        /// <returns></returns>
        public static bool Is_VK_RIGHT_Down()
        {
            int code = GetAsyncKeyState(0x26) & 0x8000;
            return code != 0;
        }

        /// <summary>
        /// 下箭头键是否按下
        /// </summary>
        /// <returns></returns>
        public static bool Is_VK_DOWN_Down()
        {
            int code = GetAsyncKeyState(0x26) & 0x8000;
            return code != 0;
        }

        /// <summary>
        /// alt是否按下 左右都能触发
        /// </summary>
        /// <returns></returns>
        public static bool IsAltDown()
        {
            int code = GetAsyncKeyState(0x12) & 0x8000;
            return code != 0;
        }
    }
}
