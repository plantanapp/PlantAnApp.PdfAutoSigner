using PdfAutoSigner.Lib.Signatures;

namespace PdfAutoSigner.LocalApi.Services
{
    public interface ISignaturesProviderService
    {
        List<IExternalSignatureWithChain> GetAvailableSignatures();
    }
}