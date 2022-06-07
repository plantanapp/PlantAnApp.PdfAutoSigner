using PdfAutoSigner.LocalApi.Config;
using System.Runtime.InteropServices;

namespace PdfAutoSigner.LocalApi.Helpers
{
    public interface IOSHelper
    {
        SupportedOS? GetOS();
    }
}