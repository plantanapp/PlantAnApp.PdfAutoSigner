using PdfAutoSigner.Lib.Signatures;

namespace PdfAutoSigner.LocalApi.Services
{
    public interface ISignerService
    {
        MemoryStream Sign(Stream inputStream, string signatureIdentifyingName, string pin);
    }
}