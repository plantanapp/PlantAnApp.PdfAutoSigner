﻿using PdfAutoSigner.Lib.Signatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PdfAutoSigner.LocalApi.Tests
{
    // Based on https://stackoverflow.com/questions/13806299/how-can-i-create-a-self-signed-certificate-using-c
    public class X509CertificateFactory
    {
        public static X509Certificate2 CreateRsaCertificate(string password)
        {
            var rsaCng = RSACng.Create();

            var req = new CertificateRequest("cn=testcert", rsaCng, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            var cert = req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(5));

            // use cert2.GetECDsaPrivateKey() to get the private key
            X509Certificate2 cert2 = new X509Certificate2(cert.Export(X509ContentType.Pfx, password), password, 
                X509KeyStorageFlags.PersistKeySet);

            return cert2;
        }
    }
}
