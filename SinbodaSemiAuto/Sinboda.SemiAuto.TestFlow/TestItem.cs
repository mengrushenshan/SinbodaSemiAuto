using Sinboda.SemiAuto.Business.Items;
using Sinboda.SemiAuto.Core.Models;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.TestFlow
{
    public class TestItem
    {
        /// <summary>
        /// X轴每个点的便宜量
        /// </summary>
        private const int XaxisOffset = 14400;

        /// <summary>
        /// Y轴每个点的便宜量
        /// </summary>
        private const int YaxisOffset = 14400;

        /// <summary>
        /// 测试点位数量
        /// </summary>
        private const int PointCount = 9;

        /// <summary>
        /// 测试流水
        /// </summary>
        public int Testid { get; set; }

        /// <summary>
        /// 测试状态
        /// </summary>
        public TestState State { get; set; }

        /// <summary>
        /// X轴坐标
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y轴坐标
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Z轴坐标
        /// </summary>
        public int Z { get; set; }

        /// <summary>
        /// 测试点位
        /// </summary>
        public List<TestPoint> points { get; set; } = new List<TestPoint>();

        /// <summary>
        /// 当前测试点
        /// </summary>
        public TestPoint CurTestPoint { get; set; } = null;

        /// <summary>
        /// 创建点位的坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="rack"></param>
        /// <param name="pos"></param>
        public void SetTestItemPos(int x, int y, int z, int rack, int pos)
        {
            X = x + (pos - 1) * XaxisOffset;
            Y = y + (rack - 1) * YaxisOffset;
            Z = z;
        }

        /// <summary>
        /// 移动到X轴位置
        /// </summary>
        /// <param name="motorId"></param>
        public void MoveTestItemXPos(int motorId)
        {
            MotorBusiness.Instance.MoveAbsolute(motorId, X);
            
        }

        /// <summary>
        /// 移动到Y轴位置
        /// </summary>
        /// <param name="motorId"></param>
        public void MoveTestItemYPos(int motorId)
        {
            MotorBusiness.Instance.MoveAbsolute(motorId, Y);
        }

        /// <summary>
        /// 移动到Z轴位置
        /// </summary>
        /// <param name="ximcArm"></param>
        public void MoveTestItemZPos(XimcArm ximcArm)
        {
            MotorBusiness.Instance.XimcMoveFast(ximcArm, Z);
        }

        /// <summary>
        /// 根据偏移量移动到Z轴位置
        /// </summary>
        /// <param name="ximcArm"></param>
        public void MoveTestItemZPosByOffset(XimcArm ximcArm, int offset)
        {
            MotorBusiness.Instance.XimcMoveFast(ximcArm, Z + offset);
        }

        /// <summary>
        /// 创建测试点
        /// </summary>
        public void CreatePoint()
        { 
            for (int i = 1; i <= PointCount; i++) 
            {
                TestPoint testPoint = new TestPoint();
                testPoint.TestNo = i;
                testPoint.Status = TestState.Untested;
                testPoint.SetTestPointPos(X, Y, Z);

                points.Add(testPoint);
            }

            if (points.Count > 0) 
            {
                CurTestPoint = points[0];
            }
        }

        public void ChannelPoint()
        {
            CurTestPoint = null;
            points.Clear();
        }
    }
}
