using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Pvcam
{
    public class DirectBitmap : IDisposable
    {
        public Bitmap Bitmap { get; private set; }
        public byte[] Bits { get; private set; }
        public bool Disposed { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        public int Stride { get; private set; }

        protected GCHandle BitsHandle { get; private set; }

        public DirectBitmap(Size size)
        {
            Init(size.Width, size.Height);
        }
        public DirectBitmap(int width, int height)
        {
            Init(width, height);
        }
        private void Init(int width, int height)
        {
            // With minor modifications, this class supports the PARGB and RGB formats.
            // The PARGB format should be the native format for drawing bitmaps on .NET
            // controls, it contains an alpha channel though. The RGB format, on the other
            // hand saves one byte per pixel but the .NET will allegedly do a conversion 
            // from PARGB to RGB before the display.
            // It really seems that the PARGB, while a bit larger, is faster. Measured
            // about 43 vs 39 display FPS when using PARGB instead of RGB.
            PixelFormat fmt = PixelFormat.Format32bppPArgb;
            switch (fmt)
            {
                case PixelFormat.Format32bppPArgb:
                    Stride = width * 4;
                    break;
                case PixelFormat.Format24bppRgb:
                    const int dstStrideAlign = 4;
                    const int dstBpp = 3;
                    Stride = (((width * dstBpp) + dstStrideAlign - 1) / dstStrideAlign) * dstStrideAlign;
                    break;
                default:
                    throw new Exception("Unsupported format");
            }

            Width = width;
            Height = height;
            Bits = new byte[Stride * height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, Stride, fmt, BitsHandle.AddrOfPinnedObject());
            // Prefill all the bits to 255. With PARGB format, we will only update the RGB bytes
            // and the alpha channel will always stay at 255. Helps a bit with performance as
            // we don't really have to overwrite the alpha channel all the time.
            for (int i = 0; i < Bits.Length; ++i)
                Bits[i] = 255;
        }

        void _processScanLine_PARGB_8(byte[] dstData, int dstStride, byte[] srcData, int srcStride, byte srcMin, byte dstMin, double factor, int y)
        {
            const int cSrcBpp = 1;
            const int cDstBpp = 4;
            for (int sIdx = y * srcStride, dIdx = y * dstStride; sIdx < (y + 1) * srcStride; sIdx += cSrcBpp, dIdx += cDstBpp)
            {
                byte pixVal = srcData[sIdx]; // Original pixel value
                // Rescale it
                pixVal = (byte)((pixVal - srcMin) * factor + dstMin);
                // Convert to B-G-R-A format
                dstData[dIdx + 0] = dstData[dIdx + 1] = dstData[dIdx + 2] = pixVal;
                //dstData[dIdx + 3] = 255;
            }
        }
        void _processScanLine_RGB_8(byte[] dstData, int dstStride, byte[] srcData, int srcStride, byte srcMin, byte dstMin, double factor, int y)
        {
            const int cSrcBpp = 1;
            const int cDstBpp = 3;
            for (int sIdx = y * srcStride, dIdx = y * dstStride; sIdx < (y + 1) * srcStride; sIdx += cSrcBpp, dIdx += cDstBpp)
            {
                byte pixVal = srcData[sIdx]; // Original pixel value
                // Rescale it
                pixVal = (byte)((pixVal - srcMin) * factor + dstMin);
                // Convert to R-G-B format
                dstData[dIdx + 0] = dstData[dIdx + 1] = dstData[dIdx + 2] = pixVal;
            }
        }
        void _processScanLine_PARGB_16(byte[] dstData, int dstStride, byte[] srcData, int srcStride, ushort srcMin, ushort dstMin, double factor, int y)
        {
            const int cSrcBpp = 2;
            const int cDstBpp = 4;
            for (int sIdx = y * srcStride, dIdx = y * dstStride; sIdx < (y + 1) * srcStride; sIdx += cSrcBpp, dIdx += cDstBpp)
            {
                // Combine the 2 bytes into one short
                ushort b0 = srcData[sIdx + 0];
                ushort b1 = srcData[sIdx + 1];
                ushort pixVal = (ushort)(b0 + (b1 << 8));
                // Rescale it
                pixVal = (ushort)((pixVal - srcMin) * factor + dstMin);
                // Convert to B-G-R-A format
                dstData[dIdx + 0] = dstData[dIdx + 1] = dstData[dIdx + 2] = (byte)pixVal;
                //dstData[dIdx + 3] = 255;
            }
        }
        void _processScanLine_RGB_16(byte[] dstData, int dstStride, byte[] srcData, int srcStride, ushort srcMin, ushort dstMin, double factor, int y)
        {
            const int cSrcBpp = 2;
            const int cDstBpp = 3;
            for (int sIdx = y * srcStride, dIdx = y * dstStride; sIdx < (y + 1) * srcStride; sIdx += cSrcBpp, dIdx += cDstBpp)
            {
                // Combine the 2 bytes into one short
                ushort b0 = srcData[sIdx + 0];
                ushort b1 = srcData[sIdx + 1];
                ushort pixVal = (ushort)(b0 + (b1 << 8));
                // Rescale it
                pixVal = (ushort)((pixVal - srcMin) * factor + dstMin);
                // Convert to R-G-B format
                dstData[dIdx + 0] = dstData[dIdx + 1] = dstData[dIdx + 2] = (byte)pixVal;
            }
        }

        /// <summary>
        /// Convert raw image buffer into 8-bit displayable image and optionally scale the contrast for better display.
        /// For the input range, use either 0-MAX_ADU (0-2047 for 12-bit, 0-65535 for 16-bit data, etc.) to not scale the image,
        /// or use the actual calculated pixel data range (e.g. 156-287) to rescale the range (e.g. the 156-287 will be rescaled to 0-255).
        /// The image range cannot be 0 (i.e. srcMin==srcMax), in such cases, the caller is responsible for disabling the scaling
        /// by setting the range to 0-MAX_ADU.
        /// </summary>
        /// <param name="srcData">Input byte array, the size must correspond to the given image size and format.</param>
        /// <param name="imageSize">Image size</param>
        /// <param name="srcFmt">Image format</param>
        /// <param name="srcMin">Input image range for contrast scaling.</param>
        /// <param name="srcMax">Input image range for contrast scaling.</param>
        /// <param name="useParallelProcessing">Enable parallel processing</param>
        public void FrameToBMP(byte[] srcData, Size imageSize, PVCAM.PL_IMAGE_FORMATS srcFmt, double srcMin, double srcMax, bool useParallelProcessing)
        {
            if (srcMax == srcMin)
                throw new Exception("Cannot contrast-scale image that has 0 input range");

            // Scaling to a displayable range of 0-255
            const double dstMin = 0;
            const double dstMax = 255;

            // Precalculated multiplier, we will need to add offset later too
            double cFactor = (dstMax - dstMin) / (srcMax - srcMin);

            // Limit the max number of threads for processing to a resonable amount
            int tN = Math.Min(Environment.ProcessorCount, 16);

            byte[] dataRgb = Bits;
            int srcWidth = imageSize.Width;
            int srcHeight = imageSize.Height;
            int dstStride = Stride;

            switch (srcFmt)
            {
                case PVCAM.PL_IMAGE_FORMATS.PL_IMAGE_FORMAT_MONO8:
                case PVCAM.PL_IMAGE_FORMATS.PL_IMAGE_FORMAT_BAYER8:
                    {
                        byte cSrcMin = (byte)srcMin;
                        byte cDstMin = (byte)dstMin;
                        const int cSrcBpp = 1; // 1 byte per pixel in source bitmap
                        int srcStride = srcWidth * cSrcBpp;
                        if (useParallelProcessing)
                        {
                            Parallel.For(0, tN, tIdx =>
                            {
                                // Every thread processes N-th line
                                for (int y = tIdx; y < srcHeight; y += tN)
                                    _processScanLine_PARGB_8(dataRgb, dstStride, srcData, srcStride, cSrcMin, cDstMin, cFactor, y);
                            });
                        }
                        else
                        {
                            for (int y = 0; y < srcHeight; ++y)
                                _processScanLine_PARGB_8(dataRgb, dstStride, srcData, srcStride, cSrcMin, cDstMin, cFactor, y);
                        }
                    }
                    break;
                case PVCAM.PL_IMAGE_FORMATS.PL_IMAGE_FORMAT_MONO16:
                case PVCAM.PL_IMAGE_FORMATS.PL_IMAGE_FORMAT_BAYER16:
                    {
                        ushort cSrcMin = (ushort)srcMin;
                        ushort cDstMin = (ushort)dstMin;
                        const int cSrcBpp = 2; // 2 bytes per pixel in source bitmap
                        int srcStride = srcWidth * cSrcBpp;

                        if (useParallelProcessing)
                        {
                            Parallel.For(0, tN, tIdx =>
                            {
                                // Every thread processes N-th line
                                for (int y = tIdx; y < srcHeight; y += tN)
                                    _processScanLine_PARGB_16(dataRgb, dstStride, srcData, srcStride, cSrcMin, cDstMin, cFactor, y);
                            });
                        }
                        else
                        {
                            for (int y = 0; y < srcHeight; ++y)
                                _processScanLine_PARGB_16(dataRgb, dstStride, srcData, srcStride, cSrcMin, cDstMin, cFactor, y);
                        }
                    }
                    break;
                default:
                    throw new Exception("Unsupported input data format");
            }
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
        }
    }
}
