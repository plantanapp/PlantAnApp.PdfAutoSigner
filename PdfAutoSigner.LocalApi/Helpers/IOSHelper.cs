using PdfAutoSigner.LocalApi.Config;
using System.Runtime.InteropServices;

namespace PdfAutoSigner.LocalApi.Helpers
{
    public interface IOSHelper
    {
        Architecture GetArchitecture();
        SupportedOS? GetOS();
    }
}