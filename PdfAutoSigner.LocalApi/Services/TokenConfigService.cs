using PdfAutoSigner.LocalApi.Config;
using PdfAutoSigner.LocalApi.Helpers;
using System.Runtime.InteropServices;

namespace PdfAutoSigner.LocalApi.Services
{
    public class TokenConfigService : ITokenConfigService
    {
        private readonly IConfiguration configuration;
        private readonly IOSHelper osHelper;
        private ILogger<TokenConfigService> logger;

        public TokenConfigService(IConfiguration configuration, IOSHelper osHelper, ILogger<TokenConfigService> logger)
        {
            this.configuration = configuration;
            this.osHelper = osHelper;
            this.logger = logger;
        }

        public List<string> GetPkcs11LibPathsByOS()
        {
            var deviceDataList = configuration.GetSection(TokenOptions.Pkcs11DevicesConfigPath).Get<List<Pkcs11DeviceData>>();
            if (deviceDataList == null)
            {
                throw new FormatException($"Could not parse the section {TokenOptions.Pkcs11DevicesConfigPath} in the token settings json file."); 
            }

            var os = osHelper.GetOS();
            if (os == null)
            {
                throw new SystemException("OS is not supported.");
            }

            var architecture = RuntimeInformation.OSArchitecture;

            var libPaths =
                from device in deviceDataList
                from osLibPath in device.Pkcs11LibPaths
                where osLibPath.OS == os && osLibPath.Architecture == architecture
                select osLibPath.LibPath;

            return libPaths.ToList();
        }

        public List<string> GetIssuerNames()
        {
            var certificateDataList = configuration.GetSection(TokenOptions.CerticatesConfigPath).Get<List<CertificateData>>();
            if (certificateDataList == null)
            {
                throw new FormatException($"Could not parse the section {TokenOptions.CerticatesConfigPath} in token settings json file.");
            }

            var certificateIssuerNames =
                from certificateData in certificateDataList
                select certificateData.CertificateIssuerName;

            return certificateIssuerNames.ToList();
        }
    }
}
