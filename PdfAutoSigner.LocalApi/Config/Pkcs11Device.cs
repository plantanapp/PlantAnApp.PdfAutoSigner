namespace PdfAutoSigner.LocalApi.Config
{
    public class Pkcs11DeviceData
    {
        public string Name { get; set; }
        public List<Pkcs11LibPathData> Pkcs11LibPaths { get; set; }
    }
}
