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

        //        附其他常用键位：
        //VK_LBUTTON 鼠标左键                      0x01
        //VK_RBUTTON 鼠标右键                      0x02
        //VK_CANCEL Ctrl + Break                  0x03
        //VK_MBUTTON 鼠标中键                      0x04

        //VK_BACK Backspace 键       0x08
        //VK_TAB Tab 键                        0x09

        //VK_RETURN 回车键                        0x0D


        //VK_SHIFT Shift 键                      0x10
        //VK_CONTROL Ctrl 键                       0x11
        //VK_MENU Alt 键                 0x12
        //VK_PAUSE Pause 键                      0x13
        //VK_CAPITAL Caps Lock 键                  0x14

        //VK_ESCAPE Esc 键                        0x1B

        //VK_SPACE 空格键         0x20
        //VK_PRIOR Page Up 键                    0x21
        //VK_NEXT Page Down 键                  0x22
        //VK_END End 键                        0x23
        //VK_HOME Home 键                       0x24
        //VK_LEFT 左箭头键                      0x25
        //VK_UP 上箭头键                      0x26
        //VK_RIGHT 右箭头键                      0x27
        //VK_DOWN 下箭头键                      0x28
        //VK_SNAPSHOT Print Screen 键               0x2C
        //VK_Insert Insert 键                     0x2D
        //VK_Delete Delete 键                     0x2E

        //'0' – '9'             数字 0 - 9                    0x30 - 0x39
        //'A' – 'Z'             字母 A - Z                    0x41 - 0x5A

        //VK_LWIN 左WinKey(104键盘才有)         0x5B
        //VK_RWIN 右WinKey(104键盘才有)         0x5C
        //VK_APPS AppsKey(104键盘才有)          0x5D

        //VK_NUMPAD0 小键盘 0 键                    0x60
        //VK_NUMPAD1 小键盘 1 键                    0x61
        //VK_NUMPAD2 小键盘 2 键                    0x62
        //VK_NUMPAD3 小键盘 3 键                    0x63
        //VK_NUMPAD4 小键盘 4 键                    0x64
        //VK_NUMPAD5 小键盘 5 键                    0x65
        //VK_NUMPAD6 小键盘 6 键                    0x66
        //VK_NUMPAD7 小键盘 7 键                    0x67
        //VK_NUMPAD8 小键盘 8 键                    0x68
        //VK_NUMPAD9 小键盘 9 键                    0x69

        //VK_F1 - VK_F24 功能键F1 – F24               0x70 - 0x87

        //VK_NUMLOCK Num Lock 键                   0x90
        //VK_SCROLL Scroll Lock 键                0x91



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
        /// shift是否按下 左右都能触发
        /// </summary>
        /// <returns></returns>
        public static bool IsShiftDown()
        {
            int code = GetAsyncKeyState(0x10) & 0x8000;
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
