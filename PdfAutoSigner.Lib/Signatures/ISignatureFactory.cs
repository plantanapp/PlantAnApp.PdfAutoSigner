namespace PdfAutoSigner.Lib.Signatures
{
    public interface ISignatureFactory
    {
        Pkcs11Signature CreatePkcs11Signature(string libraryPath, ulong slotId, string? alias = null, string? certLabel = null, string? pin = null);
        X509Certificate2Signature CreateX509Certificate2Signature(string issuerName, ulong signatureIdx, string pin);
        List<Pkcs11Signature> GetAvailablePkcs11Signatures(List<string> libraryPaths);
        List<X509Certificate2Signature> GetAvailableX509Certificate2Signatures(List<string> issuerNames);
    }
}