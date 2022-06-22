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

using iText.Signatures;
using Org.BouncyCastle.X509;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using PdfAutoSigner.Lib.Signatures;

namespace PdfAutoSigner.Lib.Signatures
{
    /// <summary>
    /// Original code obtained from https://git.itextsupport.com/projects/I5N/repos/itextsharp/browse/src/core/iTextSharp/text/pdf/security/X509Certificate2Signature.cs?at=refs/heads/develop
    /// </summary>
    public class X509Certificate2Signature: IExternalSignatureWithChain
    {
        // For now only SHA256 is supported.
        // public static readonly string X509CertHashAlgorithm = "SHA256";
        private static readonly HashAlgorithmName X509CertHashAlgorithmName = HashAlgorithmName.SHA256;

        /// <summary>
        /// The certificate with the private key
        /// </summary>
        private X509Certificate2 certificate;
        /** The hash algorithm. */
        private String hashAlgorithm;
        /** The encryption algorithm (obtained from the private key) */
        private String encryptionAlgorithm;

        private Org.BouncyCastle.X509.X509Certificate[] chain;

        /// <summary>
        /// Creates a signature using a X509Certificate2. It supports smartcards without 
        /// exportable private keys.
        /// </summary>
        /// <param name="certificate">The certificate with the private key</param>
        /// <param name="hashAlgorithm">The hash algorithm for the signature. As the Windows CAPI is used
        /// to do the signature the only hash guaranteed to exist is SHA-1</param>
        public X509Certificate2Signature(X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                throw new ArgumentException("Certificate should not be null");
            }

            if (!certificate.HasPrivateKey)
                throw new ArgumentException("No private key.");
            this.certificate = certificate;
            this.hashAlgorithm = DigestAlgorithms.GetDigest(DigestAlgorithms.GetAllowedDigest(X509CertHashAlgorithmName.Name));
            var privateKey = certificate.GetPrivateKey();
            if (privateKey is RSA)
            {
                encryptionAlgorithm = "RSA";
            }
            else if (privateKey is RSACng)
            {
                encryptionAlgorithm = "RSA";
            }
            else if (privateKey is DSA)
            {
                encryptionAlgorithm = "DSA";
            }
            else
                throw new ArgumentException("Unknown encryption algorithm " + privateKey);
        }

        public IExternalSignatureWithChain Select(string pin)
        {
            X509CertificateParser objCP = new X509CertificateParser();
            chain = new Org.BouncyCastle.X509.X509Certificate[] { objCP.ReadCertificate(certificate.RawData) };

            if (!string.IsNullOrWhiteSpace(pin))
            {
                SetPin(pin);
            }

            return this;
        }

        private X509Certificate2Signature SetPin(string pin)
        {
            var privateKey = certificate.GetPrivateKey();
            // For RSA and DSA
            if (privateKey is ICspAsymmetricAlgorithm)
            {
                var cspAsymAlg = (ICspAsymmetricAlgorithm)certificate.PrivateKey;
                CspParameters cspParameters =
                    new CspParameters
                    {
                        ProviderType = cspAsymAlg.CspKeyContainerInfo.ProviderType,
                        ProviderName = cspAsymAlg.CspKeyContainerInfo.ProviderName,
                        KeyContainerName = cspAsymAlg.CspKeyContainerInfo.KeyContainerName,
                        //KeyNumber = ((int)cspAsymAlg.CspKeyContainerInfo.KeyNumber),
                        KeyPassword = new NetworkCredential("", pin).SecurePassword,
                        Flags = CspProviderFlags.UseExistingKey | CspProviderFlags.NoPrompt
                    };

                // set modified RSA crypto provider back
                if (privateKey is RSACryptoServiceProvider)
                {
                    var newPrivateKey = new RSACryptoServiceProvider(cspParameters);
                    certificate = certificate.CopyWithPrivateKey(newPrivateKey);
                }
                else if (privateKey is DSACryptoServiceProvider)
                {
                    var newPrivateKey = new DSACryptoServiceProvider(cspParameters);
                    certificate = certificate.CopyWithPrivateKey(newPrivateKey);
                }
            }
            // Different approach is needed for RSACng
            else if (privateKey is RSACng rsaCng)
            {
                // Set the PIN, an explicit null terminator is required to this Unicode/UCS-2 string.

                byte[] propertyBytes;

                if (pin[pin.Length - 1] == '\0')
                {
                    propertyBytes = Encoding.Unicode.GetBytes(pin);
                }
                else
                {
                    propertyBytes = new byte[Encoding.Unicode.GetByteCount(pin) + 2];
                    Encoding.Unicode.GetBytes(pin, 0, pin.Length, propertyBytes, 0);
                }

                const string NCRYPT_PIN_PROPERTY = "SmartCardPin";

                CngProperty pinProperty = new CngProperty(
                    NCRYPT_PIN_PROPERTY,
                    propertyBytes,
                    CngPropertyOptions.None);

                rsaCng.Key.SetProperty(pinProperty);
            }

            return this;
        }

        public virtual byte[] Sign(byte[] message)
        {
            var privateKey = certificate.GetPrivateKey();

            if (privateKey is RSA rsa)
            {
                return rsa.SignData(message, X509CertHashAlgorithmName, RSASignaturePadding.Pkcs1);
            }
            //else if (privateKey is RSACng)
            //{
            //    RSACng rsa = (RSACng)certificate.PrivateKey;
            //    return rsa.SignData(message, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            //}
            else if (privateKey is DSA dsa)
            {
                return dsa.SignData(message, X509CertHashAlgorithmName);
            }

            return Array.Empty<byte>();
        }

        public Org.BouncyCastle.X509.X509Certificate[] GetChain()
        {
            return chain;
        }

        public string GetSignatureIdentifyingName()
        {
            return $"{certificate.Subject} - {certificate.SerialNumber}";
        }

        /**
         * Returns the hash algorithm.
         * @return  the hash algorithm (e.g. "SHA-1", "SHA-256,...")
         * @see com.itextpdf.text.pdf.security.ExternalSignature#getHashAlgorithm()
         */
        public virtual String GetHashAlgorithm()
        {
            return hashAlgorithm;
        }

        /**
         * Returns the encryption algorithm used for signing.
         * @return the encryption algorithm ("RSA" or "DSA")
         * @see com.itextpdf.text.pdf.security.ExternalSignature#getEncryptionAlgorithm()
         */
        public virtual String GetEncryptionAlgorithm()
        {
            return encryptionAlgorithm;
        }
    }
}
