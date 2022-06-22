using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace PdfAutoSigner.Lib.Signatures
{
    public static class X509Certificate2Extensions
    {
        const String RSA = "1.2.840.113549.1.1.1";
        const String DSA = "1.2.840.10040.4.1";
        const String ECC = "1.2.840.10045.2.1";

        public static AsymmetricAlgorithm? GetPrivateKey(this X509Certificate2 cert)
        {
            AsymmetricAlgorithm? key = null;
            switch (cert.PublicKey.Oid.Value)
            {
                case RSA:
                    key = cert.GetRSAPrivateKey();
                    break;
                case DSA:
                    key = cert.GetDSAPrivateKey();
                    break;
                case ECC:
                    key = cert.GetECDsaPrivateKey(); 
                    break;
            }

            return key;
        }
    }
}
