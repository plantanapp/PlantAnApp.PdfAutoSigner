using PdfAutoSigner.Lib.Signatures;

namespace PdfAutoSigner.LocalApi.Services
{
    public interface ISignerService
    {
        List<string> GetAvailableSignatureNames();
        List<IExternalSignatureWithChain> GetAvailableSignatures();
        MemoryStream Sign(Stream inputStream, string signatureIdentifyingName, string pin);
    }
}