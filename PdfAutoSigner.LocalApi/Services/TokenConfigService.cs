using Microsoft.Extensions.Options;
using PdfAutoSigner.LocalApi.Config;
using PdfAutoSigner.LocalApi.Helpers;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace PdfAutoSigner.LocalApi.Services
{
    public class TokenConfigService : ITokenConfigService
    {
        private readonly IOptionsSnapshot<TokenOptions> tokenOptionsSnapshot;
        private readonly IOSHelper osHelper;

        public TokenConfigService(IOptionsSnapshot<TokenOptions> tokenOptionsSnapshot, IOSHelper osHelper)
        {
            this.tokenOptionsSnapshot = tokenOptionsSnapshot;
            this.osHelper = osHelper;
        }

        public List<string> GetPkcs11LibPathsByOS()
        {
            var deviceDataList = tokenOptionsSnapshot.Value.Pkcs11Devices;
            if (deviceDataList == null)
            {
                throw new FormatException($"Could not parse the section {TokenOptions.Pkcs11DevicesConfigPath} in the token settings json file."); 
            }

            var os = osHelper.GetOS();
            if (os == null)
            {
                throw new SystemException("OS is not supported.");
            }

            var architecture = osHelper.GetArchitecture();

            var libPaths =
                from device in deviceDataList
                from osLibPath in device.Pkcs11LibPaths
                where osLibPath.OS == os && osLibPath.Architecture == architecture
                select osLibPath.LibPath;

            return libPaths.ToList();
        }

        public List<string> GetIssuerNames()
        {
            var certificateDataList = tokenOptionsSnapshot.Value.Certificates;
            if (certificateDataList == null)
            {
                throw new FormatException($"Could not parse the section {TokenOptions.CertificatesConfigPath} in token settings json file.");
            }

            var certificateIssuerNames =
                from certificateData in certificateDataList
                select certificateData.CertificateIssuerName;

            return certificateIssuerNames.ToList();
        }
    }
}
