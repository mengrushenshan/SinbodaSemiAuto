using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Helpers
{
    public class Utils
    {
        /// <summary>
        /// Since .NET or the Marshal assembly do not support any kind of memset on native
        /// buffers, we import one from the Windows API.
        /// </summary>
        /// <param name="dest">Destination buffer pointer</param>
        /// <param name="c">Value to fill (treated as char)</param>
        /// <param name="byteCount">Number of bytes</param>
        /// <returns></returns>
        [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr NativeMemset(IntPtr dest, int c, int byteCount);

        /// <summary>
        /// Parallelized block copy.
        /// </summary>
        /// <param name="src">Source array</param>
        /// <param name="srcOffset">Source offset</param>
        /// <param name="dst">Destination array</param>
        /// <param name="dstOffset">Destination offset</param>
        /// <param name="count">Number of items to copy</param>
        /// <param name="tN">Number of parallel threads</param>
        public static void ParallelBlockCopy(Array src, int srcOffset, Array dst, int dstOffset, int count, int tN)
        {
            int chunkSz = count / tN;
            int chunkRm = count % tN;
            System.Threading.Tasks.Parallel.For(0, tN, tIdx =>
            {
                int chunkOf = tIdx * chunkSz;
                // If this is the last thread, deal with the remainder
                if ((chunkRm != 0) && (tIdx == tN - 1))
                    chunkSz += chunkRm;

                Buffer.BlockCopy(src, srcOffset + chunkOf, dst, dstOffset + chunkOf, chunkSz);
            });
        }
        /// <summary>
        /// Parallelized Marshal.Copy
        /// </summary>
        /// <param name="source">Source native buffer</param>
        /// <param name="destination">Destination managed buffer</param>
        /// <param name="startIndex">Starting index in the destination array</param>
        /// <param name="length">Lengt of the buffer to copy</param>
        /// <param name="tN">Number of threads to use</param>
        public static void ParallelMarshalCopy(IntPtr source, byte[] destination, int startIndex, int length, int tN)
        {
            int chunkSz = length / tN;
            int chunkRm = length % tN;
            System.Threading.Tasks.Parallel.For(0, tN, tIdx =>
            {
                int chunkOf = tIdx * chunkSz;
                // If this is the last thread, deal with the remainder
                if ((chunkRm != 0) && (tIdx == tN - 1))
                    chunkSz += chunkRm;

                Marshal.Copy(source + chunkOf, destination, startIndex + chunkOf, chunkSz);
            });
        }
    }
}
