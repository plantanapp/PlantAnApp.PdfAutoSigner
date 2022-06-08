using iText.Kernel.Geom;
using PdfAutoSigner.Lib.Signatures;
using PdfAutoSigner.Lib.Signers;

namespace PdfAutoSigner.LocalApi.Services
{
    public class SignerService : ISignerService
    {
        private ITokenConfigService tokenConfigService;
        private ISignatureFactory signatureFactory;
        private IDocAutoSigner signer;
        private ILogger<SignerService> logger;

        public SignerService(ITokenConfigService tokenConfigService, ISignatureFactory signatureFactory, IDocAutoSigner signer, ILogger<SignerService> logger)
        {
            this.tokenConfigService = tokenConfigService;
            this.signatureFactory = signatureFactory;
            this.signer = signer;
            this.logger = logger;
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

        public List<string> GetAvailableSignatureNames()
        {
            var availableSignatures = GetAvailableSignatures();

            var availableSignatureNames = availableSignatures.Select(s => s.GetSignatureIdentifyingName()).ToList();
            return availableSignatureNames;
        }

        public MemoryStream Sign(Stream inputStream, string signatureIdentifyingName, string pin)
        {
            var availableSignatures = GetAvailableSignatures();

            var signature = availableSignatures.Find(s => s.GetSignatureIdentifyingName() == signatureIdentifyingName);
            if (signature == null)
            {
                logger.LogCritical($"The sign method was called with a signature identifying name {signatureIdentifyingName} that was not found");
                throw new ArgumentException($"Could not find signature with identifying name {signatureIdentifyingName}");
            }

            signature = signature.Select(pin);
            var signatureAppearance = CreateSignatureAppearanceDetails();
            var signedData = signer.Sign(inputStream, signature, signatureAppearance);
            return signedData;
        }

        private SignatureAppearanceDetails CreateSignatureAppearanceDetails()
        {
            var signatureApperance = new SignatureAppearanceDetails
            {
                Contact = "",
                Reason = "",
                Location = "",
                PageNumber = 1,
                Rectangle = new Rectangle(360, 60, 100, 40)
            };

            return signatureApperance;
        }
    }
}
