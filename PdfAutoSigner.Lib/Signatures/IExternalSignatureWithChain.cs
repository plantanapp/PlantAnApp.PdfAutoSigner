using iText.Signatures;
using Org.BouncyCastle.X509;

namespace PdfAutoSigner.Lib.Signatures
{
    public interface IExternalSignatureWithChain : IExternalSignature
    {
        X509Certificate[] GetChain();
    }
}
