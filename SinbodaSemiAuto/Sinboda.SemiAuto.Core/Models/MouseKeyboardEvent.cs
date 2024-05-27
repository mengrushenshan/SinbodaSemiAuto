using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace sin_mole_flu_analyzer.Models
{
    public class MouseEvent
    {
        /// <summary>
        /// 是否鼠标按下
        /// </summary>
        public bool IsMouseDown {  get; set; }

        /// <summary>
        /// 鼠标位置
        /// </summary>
        public Point PointNow { get; set; }

        /// <summary>
        /// 控件宽度
        /// </summary>
        public double ElementWidth {  get; set; }   
        
        /// <summary>
        /// 控件高度
        /// </summary>
        public double ElementHeight {  get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="down"></param>
        /// <param name="p"></param>
        public MouseEvent(bool down,Point p,double width, double height) 
        {
            IsMouseDown = down;
            PointNow = p;
            ElementWidth = width;
            ElementHeight = height;
        }
    }

    public class KeyBoardEvent
    {
        /// <summary>
        /// 是否键盘按下
        /// </summary>
        public bool IsKeyDown { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Key KeyCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="down"></param>
        /// <param name="p"></param>
        public KeyBoardEvent(bool down, Key p)
        {
            IsKeyDown = down;
            KeyCode = p;
        }
    }  
    
    public class MouseWheelEvent
    {
        /// <summary>
        /// 是否滚轮向下滚动
        /// </summary>
        public bool IsWheelDown { get; set; }

        /// <summary>
        /// 滚动距离
        /// </summary>
        public int Delta { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="down"></param>
        /// <param name="p"></param>
        public MouseWheelEvent(bool down, int p)
        {
            IsWheelDown = down;
            Delta = p;
        }
    }
}
