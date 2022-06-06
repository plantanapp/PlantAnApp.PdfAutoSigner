using System.Security.Cryptography.X509Certificates;

namespace PdfAutoSigner.Lib.Signatures
{
    public class SignatureFactory : ISignatureFactory
    {
        public Pkcs11Signature CreatePkcs11Signature(string libraryPath, ulong slotId, string? pin = null, string hashAlgorithm = "SHA256", string? alias = null, string? certLabel = null)
        {
            return new Pkcs11Signature(libraryPath, slotId)
                .Select(alias, certLabel, pin).SetHashAlgorithm(hashAlgorithm);
        }

        public X509Certificate2Signature CreateX509Certificate2Signature(string issuerName, ulong signatureIdx, string? pin = null, string hashAlgorithm = "SHA-256")
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.MaxAllowed);
            var certificates = store.Certificates.Where(x => x.HasPrivateKey && x.Issuer.ToLower().Contains(issuerName.ToLower())).ToArray();
            var certificate = certificates[signatureIdx];
            store.Close();

            return new X509Certificate2Signature(certificate, hashAlgorithm).Select(pin);
        }
    }
}
