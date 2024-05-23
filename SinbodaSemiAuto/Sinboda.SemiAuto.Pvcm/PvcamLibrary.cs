using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Pvcam
{
    /// <summary>
    /// This class manages initial configuration of the PVCAM library. Use
    /// this class to initialize PVCAM and list cameras.
    /// </summary>
    public class PvcamLibrary
    {
        public static bool IsInitialized { get; private set; } = false;
        public static int VersionMajor { get; private set; } = 0;
        public static int VersionMinor { get; private set; } = 0;
        public static int VersionRevision { get; private set; } = 0;

        public static void Initialize()
        {
            // initialize the library
            if (!PVCAM.pl_pvcam_init())
            {
                throw new PvcamException("Failed to initialize PVCAM", PVCAM.pl_error_code());
            }
            // Read PVCAM version
            if (!PVCAM.pl_pvcam_get_ver(out ushort versionRaw))
            {
                // This is serious error, throw.
                throw new PvcamException("Failed to read PVCAM version", PVCAM.pl_error_code());
            }
            VersionMajor = (versionRaw & 0xFF00) >> 8;
            VersionMinor = (versionRaw & 0x00F0) >> 4;
            VersionRevision = (versionRaw & 0x000F);

            IsInitialized = true;
        }
        public static void Uninitialize()
        {
            if (!PVCAM.pl_pvcam_uninit())
            {
                throw new PvcamException("Failed to uninitialize PVCAM", PVCAM.pl_error_code());
            }
            IsInitialized = false;
        }
        /// <summary>
        /// Returns a list of camera names. The camera name is then used when opening a camera.
        /// </summary>
        /// <returns>List of available cameras in the system</returns>
        public static List<string> ListCameras()
        {
            // Currently, PVCAM refreshes the list of connected cameras upon PVCAM initialization.
            // If dynamic discovery is required, PVCAM will need to be uninitialized and initialized
            // before every call to ListCameras().

            List<string> list = new List<string>();

            // Read number of avaialable (connected) cameras
            short cameraCount = 0;
            if (!PVCAM.pl_cam_get_total(out cameraCount))
            {
                throw new PvcamException("Failed to read number of available cameras", PVCAM.pl_error_code());
            }

            // Read string descriptions of each camera, these strings are later used to to open each camera
            for (short camIndex = 0; camIndex < cameraCount; ++camIndex)
            {
                StringBuilder cameraName = new StringBuilder(PVCAM.CAM_NAME_LEN);
                if (!PVCAM.pl_cam_get_name(camIndex, cameraName))
                {
                    throw new PvcamException("Failed to read camera name", PVCAM.pl_error_code());
                }

                list.Add(cameraName.ToString());
            }

            return list;
        }
    }
}
