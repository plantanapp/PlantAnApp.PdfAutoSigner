using PdfAutoSigner.Lib.Signatures;

namespace PdfAutoSigner.Lib.Signers
{
    public interface IDocAutoSigner
    {
        MemoryStream Sign(Stream inputStream, IExternalSignatureWithChain externalSignature, SignatureAppearanceDetails signatureAppearanceDetails);
    }
}