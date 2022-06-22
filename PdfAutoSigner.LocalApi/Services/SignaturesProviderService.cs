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

using PdfAutoSigner.Lib.Signatures;

namespace PdfAutoSigner.LocalApi.Services
{
    public class SignaturesProviderService : ISignaturesProviderService
    {
        private ITokenConfigService tokenConfigService;
        private ISignatureFactory signatureFactory;

        public SignaturesProviderService(ITokenConfigService tokenConfigService, ISignatureFactory signatureFactory)
        {
            this.tokenConfigService = tokenConfigService;
            this.signatureFactory = signatureFactory;
        }

        public List<IExternalSignatureWithChain> GetAvailableSignatures()
        {
            var availableSignatures = new List<IExternalSignatureWithChain>();

            var pkcs11LibPaths = tokenConfigService.GetPkcs11LibPathsByOS();
            var availablePkcs11Signatures = signatureFactory.GetAvailablePkcs11Signatures(pkcs11LibPaths);
            availableSignatures.AddRange(availablePkcs11Signatures);

            var certIssuerNames = tokenConfigService.GetIssuerNames();
            var availableCertSignatures = signatureFactory.GetAvailableX509Certificate2Signatures(certIssuerNames);
            availableSignatures.AddRange(availableCertSignatures);

            return availableSignatures;
        }
    }
}
