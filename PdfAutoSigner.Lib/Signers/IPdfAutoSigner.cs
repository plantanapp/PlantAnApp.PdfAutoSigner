using PdfAutoSigner.Lib.Signatures;

namespace PdfAutoSigner.Lib.Signers
{
    public interface IPdfAutoSigner
    {
        MemoryStream Sign(Stream inputStream, IExternalSignatureWithChain externalSignature, SignatureAppearanceDetails signatureAppearanceDetails);
    }
}