using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Pvcam
{
    /// <summary>
    /// This class calculates and holds basic statistics for integer data.
    /// The class can also merge partial results, used when calculating
    /// statistics in parallel.
    /// </summary>
    public class ImageStats
    {
        public ImageStats()
        {
            Reset();
        }
        public void Reset()
        {
            Min = long.MaxValue;
            Max = long.MinValue;
            Count = 0;
            Sum = 0;
        }
        public void Push(int num)
        {
            Min = Math.Min(Min, num);
            Max = Math.Max(Max, num);
            Sum += num;
            Count++;
        }
        public void Push(long num)
        {
            Min = Math.Min(Min, num);
            Max = Math.Max(Max, num);
            Sum += num;
            Count++;
        }
        public void Push(ImageStats other)
        {
            Min = Math.Min(Min, other.Min);
            Max = Math.Max(Max, other.Max);
            Sum += other.Sum;
            Count += other.Count;
        }

        public double Mean { get { return (double)Sum / Count; } }
        public long Min { get; private set; }
        public long Max { get; private set; }
        public long Sum { get; private set; }
        public long Count { get; private set; }
    }
}
