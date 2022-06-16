using PdfAutoSigner.Lib.Signatures;

namespace PdfAutoSigner.LocalApi.Services
{
    public class SignaturesProviderService : ISignaturesProviderService
    {
        private ITokenConfigService tokenConfigService;
        private ISignatureFactory signatureFactory;

        public SignaturesProviderService(ITokenConfigService tokenConfigService, ISignatureFactory signatureFactory)
        {
            this.tokenConfigService = tokenConfigService;
            this.signatureFactory = signatureFactory;
        }

        public List<IExternalSignatureWithChain> GetAvailableSignatures()
        {
            var availableSignatures = new List<IExternalSignatureWithChain>();

            var pkcs11LibPaths = tokenConfigService.GetPkcs11LibPathsByOS();
            var availablePkcs11Signatures = signatureFactory.GetAvailablePkcs11Signatures(pkcs11LibPaths);
            availableSignatures.AddRange(availablePkcs11Signatures);

            var certIssuerNames = tokenConfigService.GetIssuerNames();
            var availableCertSignatures = signatureFactory.GetAvailableX509Certificate2Signatures(certIssuerNames);
            availableSignatures.AddRange(availableCertSignatures);

            return availableSignatures;
        }
    }
}
