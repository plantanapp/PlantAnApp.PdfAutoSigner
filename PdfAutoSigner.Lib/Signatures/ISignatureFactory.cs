namespace PdfAutoSigner.Lib.Signatures
{
    public interface ISignatureFactory
    {
        Pkcs11Signature CreatePkcs11Signature(string libraryPath, ulong slotId, string hashAlgorithm = "SHA256", string? alias = null, string? certLabel = null, string? pin = null);
        X509Certificate2Signature CreateX509Certificate2Signature(string issuerName, ulong signatureIdx, string hashAlgorithm, string pin);
    }
}