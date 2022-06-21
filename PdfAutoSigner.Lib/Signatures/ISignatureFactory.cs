namespace PdfAutoSigner.Lib.Signatures
{
    public interface ISignatureFactory
    {
        List<Pkcs11Signature> GetAvailablePkcs11Signatures(List<string> libraryPaths);
        List<X509Certificate2Signature> GetAvailableX509Certificate2Signatures(List<string> issuerNames);
    }
}