using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sinboda.Framework.Communication.Utils
{
    public delegate void OnThread();

    public class EventThread
    {
        public enum ObjLevel
        {
            High,
            Normal,
            Low
        };

        #region 成员变量
        protected BlockingCollection<object> highQueue = new BlockingCollection<object>();
        protected BlockingCollection<object> normalQueue = new BlockingCollection<object>();
        protected BlockingCollection<object> lowQueue = new BlockingCollection<object>();

        protected ManualResetEvent resetEvent = new ManualResetEvent(true);

        protected Thread thread = null;
        protected OnThread onThread;
        #endregion

        public EventThread(OnThread onThread, ThreadPriority threadPriority = ThreadPriority.Normal)
        {
            this.onThread = onThread;

            StartThread(threadPriority);
        }

        /// <summary>
        /// 启动发送线程
        /// </summary>
        private void StartThread(ThreadPriority threadPriority)
        {
            if (thread != null && thread.IsAlive)
            {
                return;
            }

            thread = new Thread(new ThreadStart(onThread));
            thread.IsBackground = true;
            thread.Priority = threadPriority;
            thread.Start();
        }

        /// <summary>
        /// 向发送队列中添加指令
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Enqueue(object obj, ObjLevel level = ObjLevel.Normal)
        {
            if (thread == null || !thread.IsAlive)
            {
                return false;
            }

            switch (level)
            {
                case ObjLevel.Normal:
                    normalQueue.Add(obj);
                    resetEvent.Set();
                    return true;

                case ObjLevel.High:
                    highQueue.Add(obj);
                    resetEvent.Set();
                    return true;

                case ObjLevel.Low:
                    lowQueue.Add(obj);
                    resetEvent.Set();
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// 从发送队列中获取发送指令
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool Dequeue(out object obj)
        {
            resetEvent.Reset();

            while (!GetObj(out obj))
            {
                resetEvent.WaitOne();
            }

            return true;
        }

        public static bool Dequeue(EventThread umThread, out object obj)
        {
            try
            {
                return umThread.Dequeue(out obj);
            }
            catch (Exception e)
            {
                obj = null;
                return true;
            }
        }

        public void Clear()
        {
            // 清空队列
            while (highQueue.Count > 0)
            {
                highQueue.Take();
            }
            while (normalQueue.Count > 0)
            {
                normalQueue.Take();
            }
            while (lowQueue.Count > 0)
            {
                lowQueue.Take();
            }
        }

        private bool GetObj(out object obj)
        {
            if (highQueue.TryTake(out obj, 10))
            {
                return true;
            }

            if (normalQueue.TryTake(out obj, 10))
            {
                return true;
            }

            if (lowQueue.TryTake(out obj, 10))
            {
                return true;
            }

            return false;
        }
    }
}
