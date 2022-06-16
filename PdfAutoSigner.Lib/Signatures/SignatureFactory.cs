using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using System.Security.Cryptography.X509Certificates;

namespace PdfAutoSigner.Lib.Signatures
{
    public class SignatureFactory : ISignatureFactory
    {
        // For now only SHA256 is supported.
        public static readonly string Pkcs11HashAlgorithm = "SHA256";
        public static readonly string X509CertHashAlgorithm = "SHA-256";

        private ILogger<SignatureFactory> logger;

        public SignatureFactory(ILogger<SignatureFactory>? logger = null)
        {
            this.logger = logger ?? NullLogger<SignatureFactory>.Instance;
        }

        public Pkcs11Signature CreatePkcs11Signature(string libraryPath, ulong slotId, string? pin = null, string? alias = null, string? certLabel = null)
        {
            var factories = new Pkcs11InteropFactories();
            var pkcs11Library = factories.Pkcs11LibraryFactory.LoadPkcs11Library(factories, libraryPath, AppType.MultiThreaded);
            // TODO: Handle case where nothing is found
            var slot = pkcs11Library.GetSlotList(SlotsType.WithTokenPresent).Find(slot => slot.SlotId == slotId);

            var pkcs11Signature = new Pkcs11Signature(slot);
            pkcs11Signature.Select(alias, certLabel, pin);
            pkcs11Signature.SetHashAlgorithm(Pkcs11HashAlgorithm);
            return pkcs11Signature;
        }

        public X509Certificate2Signature CreateX509Certificate2Signature(string issuerName, ulong signatureIdx, string? pin = null)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.MaxAllowed);
            var certificates = store.Certificates.Where(x => x.HasPrivateKey && x.Issuer.ToLower().Contains(issuerName.ToLower())).ToArray();
            var certificate = certificates[signatureIdx];
            store.Close();

            var x509Certsignature = new X509Certificate2Signature(certificate, X509CertHashAlgorithm);
            x509Certsignature.Select(pin);
            return x509Certsignature;
        }

        /// <summary>
        /// Need to call .Select() on the final signatures.
        /// </summary>
        /// <param name="libraryPaths"></param>
        /// <returns></returns>
        public List<Pkcs11Signature> GetAvailablePkcs11Signatures(List<string> libraryPaths)
        {
            var pkcs11Signatures = new List<Pkcs11Signature>();
            foreach (var libraryPath in libraryPaths)
            {
                // Attempt to get the correct signatures
                var pkcs11Factories = new Pkcs11InteropFactories();
                IPkcs11Library? pkcs11Library = null;
                try
                {
                    pkcs11Library = pkcs11Factories.Pkcs11LibraryFactory.LoadPkcs11Library(pkcs11Factories, libraryPath, AppType.MultiThreaded);
                }
                catch (Exception ex)
                {
                    logger.LogDebug($"Could not find PKCS11 library {libraryPath}");
                    continue;
                }

                List<ISlot>? slots = null;
                try
                {
                    slots = pkcs11Library.GetSlotList(SlotsType.WithTokenPresent);
                }
                catch (Exception ex)
                {
                    logger.LogDebug($"Could not find any token that uses PKCS11 library {libraryPath}");
                    continue;
                }

                foreach (var slot in slots ?? new List<ISlot>())
                {
                    logger.LogInformation($"Found token using library {libraryPath} on slot {slot.SlotId}");
                    var pkcs11Signature = new Pkcs11Signature(slot);
                    //var pkcs11Signature = new Pkcs11Signature(libraryPath, slot.SlotId).SetHashAlgorithm(Pkcs11HashAlgorithm);
                    pkcs11Signatures.Add(pkcs11Signature);
                }
            }

            return pkcs11Signatures;
        }

        public List<X509Certificate2Signature> GetAvailableX509Certificate2Signatures(List<string> issuerNames)
        {
            var x509Certificate2Signatures = new List<X509Certificate2Signature>();
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.MaxAllowed);
            foreach (var issuerName in issuerNames)
            {
                var certificates = store.Certificates.Where(x => x.HasPrivateKey && x.Issuer.ToLower().Contains(issuerName.ToLower()));

                foreach (var cert in certificates)
                {
                    logger.LogInformation($"Found certificate {cert.FriendlyName} for issuer {issuerNames}");
                    var x509Certificate2Signature = new X509Certificate2Signature(cert, X509CertHashAlgorithm);
                    x509Certificate2Signatures.Add(x509Certificate2Signature);
                }
            }
            
            store.Close();

            return x509Certificate2Signatures;
        }
    }
}
