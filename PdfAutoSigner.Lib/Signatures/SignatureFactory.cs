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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using System.Security.Cryptography.X509Certificates;

namespace PdfAutoSigner.Lib.Signatures
{
    public class SignatureFactory : ISignatureFactory
    {
        
        

        private readonly ILogger<SignatureFactory> logger;

        public SignatureFactory(ILogger<SignatureFactory>? logger = null)
        {
            this.logger = logger ?? NullLogger<SignatureFactory>.Instance;
        }

        /// <summary>
        /// Need to call .Select() on the final signatures.
        /// </summary>
        /// <param name="libraryPaths"></param>
        /// <returns></returns>
        public List<Pkcs11Signature> GetAvailablePkcs11Signatures(List<string> libraryPaths)
        {
            var pkcs11Signatures = new List<Pkcs11Signature>();
            foreach (var libraryPath in libraryPaths)
            {
                // Attempt to get the correct signatures
                var pkcs11Factories = new Pkcs11InteropFactories();
                IPkcs11Library? pkcs11Library = null;
                try
                {
                    pkcs11Library = pkcs11Factories.Pkcs11LibraryFactory.LoadPkcs11Library(pkcs11Factories, libraryPath, AppType.MultiThreaded);
                }
                catch (Exception)
                {
                    logger.LogDebug($"Could not find PKCS11 library {libraryPath}");
                    continue;
                }

                List<ISlot>? slots = null;
                try
                {
                    slots = pkcs11Library.GetSlotList(SlotsType.WithTokenPresent);
                }
                catch (Exception)
                {
                    logger.LogDebug($"Could not find any token that uses PKCS11 library {libraryPath}");
                    continue;
                }

                foreach (var slot in slots ?? new List<ISlot>())
                {
                    logger.LogInformation($"Found token using library {libraryPath} on slot {slot.SlotId}");
                    var pkcs11Signature = new Pkcs11Signature(slot);
                    pkcs11Signatures.Add(pkcs11Signature);
                }
            }

            return pkcs11Signatures;
        }

        public List<X509Certificate2Signature> GetAvailableX509Certificate2Signatures(List<string> issuerNames)
        {
            var x509Certificate2Signatures = new List<X509Certificate2Signature>();
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.MaxAllowed);
            foreach (var issuerName in issuerNames)
            {
                var certificates = store.Certificates.Where(x => x.HasPrivateKey && x.Issuer.ToLower().Contains(issuerName.ToLower()));

                foreach (var cert in certificates)
                {
                    logger.LogInformation($"Found certificate {cert.FriendlyName} for issuer {issuerNames}");
                    var x509Certificate2Signature = new X509Certificate2Signature(cert);
                    x509Certificate2Signatures.Add(x509Certificate2Signature);
                }
            }
            
            store.Close();

            return x509Certificate2Signatures;
        }
    }
}
