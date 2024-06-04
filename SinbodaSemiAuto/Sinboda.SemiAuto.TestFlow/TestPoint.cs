﻿using Sinboda.SemiAuto.Business.Items;
using Sinboda.SemiAuto.Core.Models;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.TestFlow
{
    public class TestPoint
    {
        /// <summary>
        /// X轴每个点的便宜量
        /// </summary>
        private const int XaxisOffset = 3600;

        /// <summary>
        /// Y轴每个点的便宜量
        /// </summary>
        private const int YaxisOffset = 3600;

        /// <summary>
        /// 测试编号
        /// </summary>
        public int TestNo {  get; set; }

        /// <summary>
        /// 测试状态
        /// </summary>
        public TestState Status { get; set; }

        /// <summary>
        /// X轴位置
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y轴位置
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Z轴位置
        /// </summary>
        public int Z { get; set; }

        /// <summary>
        /// 开始采集图像
        /// </summary>
        public void StartAcquiringImage()
        {

        }

        /// <summary>
        /// 停止采集图像
        /// </summary>
        public void StopAcquiringImage()
        {

        }

        /// <summary>
        /// 设置测试点坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void SetTestPointPos(int x, int y, int z)
        {
            Z = z;
            X = XaxisPos(x);
            Y = YaxisPos(y);
        }

        /// <summary>
        /// 设置x轴对应位置
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private int XaxisPos(int x)
        {
            int result = 0;

            switch (TestNo)
            {
                case 1:
                case 4:
                case 7:
                    {
                        result = x - XaxisOffset;
                    }
                    break;
                case 2:
                case 5:
                case 8:
                    {
                        result = x;
                    }
                    break;
                case 3:
                case 6:
                case 9:
                    {
                        result = x + XaxisOffset;
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// 设置y轴对应位置
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        private int YaxisPos(int y)
        {
            int result = 0;

            switch (TestNo)
            {
                case 1:
                case 2:
                case 3:
                    {
                        result = y - YaxisOffset;
                    }
                    break;
                case 4:
                case 5:
                case 6:
                    {
                        result = y;
                    }
                    break;
                case 7:
                case 8:
                case 9:
                    {
                        result = y + YaxisOffset;
                    }
                    break;
            }
            return result;
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
    }
}
