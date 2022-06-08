using iText.Signatures;
using Org.BouncyCastle.X509;

namespace PdfAutoSigner.Lib.Signatures
{
    public interface IExternalSignatureWithChain : IExternalSignature
    {
        // Build an unique name for each signature
        string GetSignatureIdentifyingName();
        X509Certificate[] GetChain();

        IExternalSignatureWithChain Select(string pin);
    }
}
