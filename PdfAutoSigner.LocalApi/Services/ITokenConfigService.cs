namespace PdfAutoSigner.LocalApi.Services
{
    public interface ITokenConfigService
    {
        List<string> GetIssuerNames();
        List<string> GetPkcs11LibPathsByOS();
    }
}