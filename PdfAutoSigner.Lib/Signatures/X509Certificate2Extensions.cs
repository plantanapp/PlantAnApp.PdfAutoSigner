// PdfAutoSigner signs PDF files automatically using a hardware security module. 
// Copyright (C) Plant An App
//  
// This program is free software: you can redistribute it and/or modify 
// it under the terms of the GNU Affero General Public License as 
// published by the Free Software Foundation, either version 3 of the 
// License, or (at your option) any later version. 
//  
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY

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
