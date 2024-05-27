using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ximc;

namespace Sinboda.SemiAuto.Core.Models
{
    [Serializable]
    public class XimcArmsData
    {
        [XmlElement("XimcArm")]
        public List<XimcArm> XimcArms { get; set; } = new List<XimcArm>();
    }

    [Serializable]
    public class XimcArm : ObservableObject
    {
        /// <summary>
        /// 机械臂名称
        /// </summary>
        [XmlAttribute("CtrlName")]
        public SerType CtrlName
        {
            get;
            set;
        }

        /// <summary>
        /// 描述信息
        /// </summary>
        [XmlAttribute("Description")]
        public string Description { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        [XmlAttribute("Originption")]
        public int Originption { get; set; }


        /// <summary>
        /// 机械臂id
        /// </summary>
        [XmlIgnore]
        public int DeveiceId { get; set; } = -1;

        /// <summary>
        /// 速度 steps/s
        /// </summary>
        [XmlIgnore]
        public uint Speed { get; set; }

        /// <summary>
        /// 加速度 steps/s^2
        /// </summary>
        [XmlIgnore]
        public uint Accel { get; set; }

        /// <summary>
        /// 加速度 steps/s^2
        /// </summary>
        [XmlIgnore]
        public int Postion { get; set; }

        /// <summary>
        /// 减速度 steps/s^2
        /// </summary>
        [XmlIgnore]
        public uint Decel { get; set; }

        /// <summary>
        /// 设置数据 最高速度4000step 
        /// </summary>
        [XmlIgnore]
        public move_settings_t Move_Setting { get; set; }

        /// <summary>
        /// 控制名称
        /// </summary>
        [XmlIgnore]
        public controller_name_t Controller_Name_T { get; set; }

        /// <summary>
        /// 状态数据
        /// </summary>
        [XmlIgnore]
        public Move_Status MoveStatus
        {
            get
            {
                return _MoveStatus;
            }
            set
            {
                //封装了通知和值传递 更加简便
                Set(ref _MoveStatus, value);
            }
        }
        private Move_Status _MoveStatus;

        public override string ToString()
        {
            return CtrlName.ToString();
        }
    }

    /// <summary>
    /// 电机类型
    /// </summary>
    public enum SerType : byte
    {
        //无名称
        None,
        //左侧x电机
        Left_X,
        //左侧y电机
        Left_Y,
        //左侧z电机
        Left_Z,
        //右侧x电机
        Right_X,
        //右侧y电机
        Right_Y,
        //右侧z电机
        Right_Z
    }

    /// <summary>
    /// 当前电机状态
    /// </summary>
    public class Move_Status
    {
        /// <summary>
        /// 当前位置
        /// </summary>
        public int CurPosition { get; set; }

        /// <summary>
        /// 当前电机轴速度 steps/s
        /// </summary>
        public int CurSpeed { get; set; }

    }
}
