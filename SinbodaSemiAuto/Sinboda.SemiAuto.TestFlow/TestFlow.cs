using GalaSoft.MvvmLight.Messaging;
using OpenCvSharp;
using Sinboda.Framework.Common;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.SemiAuto.Business.Items;
using Sinboda.SemiAuto.Business.Samples;
using Sinboda.SemiAuto.Core.Helpers;
using Sinboda.SemiAuto.Core.Models;
using Sinboda.SemiAuto.Core.Models.Common;
using Sinboda.SemiAuto.Core.Resources;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.TestFlow
{
    public class TestFlow : TBaseSingleton<TestFlow>
    {
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);
        /// <summary>
        /// 等待拍照结束通知
        /// </summary>
        private AutoResetEvent AcquiringImageFinish = new AutoResetEvent(false);

        /// <summary>
        /// 初始化完成标志
        /// </summary>
        private bool isInitComplete = false;

        /// <summary>
        /// 测试流水每次加一，关机后更新
        /// </summary>
        private int testId = 0;

        /// <summary>
        /// 测试样本列表
        /// </summary>
        private List<Sin_Sample> SampleList = new List<Sin_Sample>();

        /// <summary>
        /// 测试板号
        /// </summary>
        public int BoardId { get; set; }

        /// <summary>
        /// 基础位置X
        /// </summary>
        public int X {  get; set; }

        /// <summary>
        /// 基础位置Y
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// 基础位置Z
        /// </summary>
        public int Z { get; set; }

        /// <summary>
        /// 当前测试
        /// </summary>
        public TestItem CurTestItem { get; set; } = null;

        /// <summary>
        /// 测试队列
        /// </summary>
        public List<TestItem> Items { get; set; } = new List<TestItem>();

        /// <summary>
        /// X轴电机
        /// </summary>
        public Sin_Motor XAxisMotor { get; set; }

        /// <summary>
        /// Y轴电机
        /// </summary>
        public Sin_Motor YAxisMotor { get; set; }

        /// <summary>
        /// Z轴电机
        /// </summary>
        public XimcArm ZAxisMotor { get; set; }

        /// <summary>
        /// 测试流程初始化
        /// </summary>
        private bool Init()
        {
            AcquiringImageFinish.Reset();
            DateTime today = DateTime.Now.Date;
            DateTime tomorrow = DateTime.Now.AddDays(1).Date;
            if (isInitComplete)
            {
                return true;
            }

            var boardList = BoardBusiness.Instance.GetBoardListByBoardId(BoardId);
            if (boardList == null || boardList.Count == 0)
            {
                return false;
            }

            var sampleList = SampleBusiness.Instance.GetSampleListByPredicate(o => o.Sample_date >= today && o.Sample_date < tomorrow && o.Test_state != TestState.Complete);

            if (sampleList == null || sampleList.Count == 0) 
            {
                return false;
            }

            SampleList = sampleList.OrderBy(p => p.RackDish).ThenBy(q => q.Position).ToList();

            //启动测试时，实时获取电机所在位置
            ResMotorStatus xStatus = MotorBusiness.Instance.GetMotorStatus((int)XAxisMotor.MotorId);
            ResMotorStatus yStatus = MotorBusiness.Instance.GetMotorStatus((int)YAxisMotor.MotorId);
            MotorBusiness.Instance.SetXimcStatus(ZAxisMotor);

            Z = ZAxisMotor.TargetPos;

            if (xStatus != null)
            {
                X = xStatus.CurrPos;
            }
            else
            {
                return false ;
            }

            if (yStatus != null)
            {
                Y = yStatus.CurrPos;
            }
            else
            {
                return false;
            }

            Messenger.Default.Register<object>(this, MessageToken.AcquiringImageComplete, ReceiveImageComplete);

            return isInitComplete = true;
        }

        public void SetMotorObj(Sin_Motor xAxis, Sin_Motor yAxis, XimcArm zAxis)
        {
            XAxisMotor = xAxis;
            YAxisMotor = yAxis;
            ZAxisMotor = zAxis;
        }

        /// <summary>
        /// 根据1位置创建测试缓存
        /// </summary>
        public bool CreateTest()
        {
            if (!Init())
            {
                return false;
            }

            string samplePath = MapPath.TifPath + "Result\\" + $"{SampleList.First().TestResult.Test_file_name}\\";
            if (!Directory.Exists(samplePath))
            {
                Directory.CreateDirectory(samplePath);
            }

            foreach (var sampleItem in SampleList)
            {
                string fileName = sampleItem.RackDish + "_" + sampleItem.Position;
                TestItem testItem = new TestItem();
                testItem.ItemSample = sampleItem;
                testItem.Testid = ++testId;
                testItem.State = TestState.Untested;
                testItem.SetTestItemPos(X, Y, Z, sampleItem.RackDish, sampleItem.Position);
                testItem.CreatePoint(fileName, samplePath);

                Items.Add(testItem);
            }

            if (Items.Count == 0)
            {
                return false;
            }
            else
            {
                CurTestItem = Items[0];

                if (CurTestItem.X != X && CurTestItem.Y != Y)
                {
                    MoveTestItemPos();
                }
            }

            return true;
        }

        /// <summary>
        /// 移动测试位置
        /// </summary>
        public void MoveTestItemPos()
        {
            CurTestItem.MoveTestItemXPos((int)XAxisMotor.MotorId);
            CurTestItem.MoveTestItemYPos((int)YAxisMotor.MotorId);
        }

        /// <summary>
        /// 移动测试点
        /// </summary>
        public void MoveTestPointPos()
        {
            //移动xy
            CurTestItem.CurTestPoint.MoveTestItemXPos((int)XAxisMotor.MotorId);
            CurTestItem.CurTestPoint.MoveTestItemYPos((int)YAxisMotor.MotorId);
        }

        /// <summary>
        /// 启动测试流程
        /// </summary>
        public void ChangeNextItem()
        {
            
        }

        /// <summary>
        /// 测试流程
        /// </summary>
        public void StartItemTest()
        {
            foreach (var item in Items)
            {
                if (item.State == TestState.Complete)
                {
                    continue;
                }

                if (CurTestItem.State == TestState.Complete)
                {
                    CurTestItem = item;
                }

                if (CurTestItem.X != X && CurTestItem.Y != Y)
                {
                    MoveTestItemPos();
                }

                PointAcquiringImage();

                //点位测试完成后，移动到下一个点
                if (CurTestItem.points.Where(o => o.Status == TestState.Complete).Count() == CurTestItem.points.Count)
                {
                    CurTestItem.State = TestState.Complete;
                    string rack = CurTestItem.ItemSample.RackDish;
                    int pos = CurTestItem.ItemSample.Position;
                    Task.Run(async () => {
                        await semaphoreSlim.WaitAsync();
                        //AnalysisHelper.Instance.Init();
                        AnalysisHelper.Instance.Analysis(CurTestItem.ItemSample.TestResult, rack.ToCharArray()[0], pos);
                        //AnalysisHelper.Instance.Shutdown();
                        semaphoreSlim.Release();
                    });
                    
                    ChangeNextItem();
                }
            }
        }

        /// <summary>
        /// 9点数据采集
        /// </summary>
        private void PointAcquiringImage()
        {
            //同一孔位9点测试
            List<TestPoint> points = CurTestItem.points.Where(o => o.Status == TestState.Untested).ToList();

            foreach (var point in points)
            {
                CurTestItem.CurTestPoint = point;
                CurTestItem.CurTestPoint.Status = TestState.Testing;
                MoveTestPointPos();
                //开始拍照
                CurTestItem.CurTestPoint.StartAcquiringImage();
                //100张图曝光30us，增加1秒超时时间
                if (AcquiringImageFinish.WaitOne(35000))
                {
                    AcquiringImageFinish.Reset();
                    //等待记录100张图像
                    CurTestItem.CurTestPoint.Status = TestState.Complete;
                }
                else
                {
                    NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "图像采集失败"));
                    break;
                }

            }
        }

        public void CannelTest()
        {
            Messenger.Default.Unregister<object>(this, MessageToken.AcquiringImageComplete, ReceiveImageComplete);
            CurTestItem.ChannelPoint();
            CurTestItem = null;
            isInitComplete = false;
            SampleList.Clear();
            XAxisMotor = null;
            YAxisMotor = null;
            ZAxisMotor = null;
        }

        public void ReceiveImageComplete(object obj)
        {
            AcquiringImageFinish.Set();
        }
    }
}
