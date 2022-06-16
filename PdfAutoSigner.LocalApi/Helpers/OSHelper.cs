using PdfAutoSigner.LocalApi.Config;
using System.Runtime.InteropServices;

namespace PdfAutoSigner.LocalApi.Helpers
{
    public class OSHelper : IOSHelper
    {
        public SupportedOS? GetOS()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return SupportedOS.Windows;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return SupportedOS.MacOS;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return SupportedOS.Linux;
            }

            return null;
        }

        public Architecture GetArchitecture()
        {
            return RuntimeInformation.OSArchitecture;
        }
    }
}
