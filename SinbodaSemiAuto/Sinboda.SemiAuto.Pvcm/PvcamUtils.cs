using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Pvcam
{
    public class PvcamUtils
    {
        static public int GetImageFormatBytesPerPixel(PVCAM.PL_IMAGE_FORMATS fmt)
        {
            switch (fmt)
            {
                case PVCAM.PL_IMAGE_FORMATS.PL_IMAGE_FORMAT_MONO16:
                case PVCAM.PL_IMAGE_FORMATS.PL_IMAGE_FORMAT_BAYER16:
                    return 2;
                case PVCAM.PL_IMAGE_FORMATS.PL_IMAGE_FORMAT_MONO8:
                case PVCAM.PL_IMAGE_FORMATS.PL_IMAGE_FORMAT_BAYER8:
                    return 1;
                case PVCAM.PL_IMAGE_FORMATS.PL_IMAGE_FORMAT_MONO24:
                case PVCAM.PL_IMAGE_FORMATS.PL_IMAGE_FORMAT_BAYER24:
                    return 3;
                case PVCAM.PL_IMAGE_FORMATS.PL_IMAGE_FORMAT_MONO32:
                case PVCAM.PL_IMAGE_FORMATS.PL_IMAGE_FORMAT_BAYER32:
                    return 4;
                case PVCAM.PL_IMAGE_FORMATS.PL_IMAGE_FORMAT_RGB24:
                    return 3;
                case PVCAM.PL_IMAGE_FORMATS.PL_IMAGE_FORMAT_RGB48:
                    return 6;
                case PVCAM.PL_IMAGE_FORMATS.PL_IMAGE_FORMAT_RGB72:
                    return 9;
            }
            throw new Exception(string.Format("Unsupported PVCAM image format {0}", fmt.ToString()));
        }
    }
}
