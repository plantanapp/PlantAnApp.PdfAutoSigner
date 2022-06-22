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

using iText.Kernel.Geom;
using PdfAutoSigner.Lib.Signatures;
using PdfAutoSigner.Lib.Signers;

namespace PdfAutoSigner.LocalApi.Services
{
    public class SignerService : ISignerService
    {
        private ISignaturesProviderService signaturesProviderService;
        private IDocAutoSigner signer;

        public SignerService(ISignaturesProviderService signaturesProviderService, IDocAutoSigner signer)
        {
            this.signaturesProviderService = signaturesProviderService;
            this.signer = signer;
        }

        public MemoryStream Sign(Stream inputStream, string signatureIdentifyingName, string pin)
        {
            var availableSignatures = signaturesProviderService.GetAvailableSignatures();

            var signature = availableSignatures.Find(s => s.GetSignatureIdentifyingName() == signatureIdentifyingName);
            if (signature == null)
            {
                throw new ApplicationException($"Could not find a signature with identifying name {signatureIdentifyingName}.");
            }

            try
            {
                signature = signature.Select(pin);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error accessing the signature {signature.GetSignatureIdentifyingName()} with the given pin.", ex);
            }

            var signatureAppearance = CreateSignatureAppearanceDetails();
            var signedData = signer.Sign(inputStream, signature, signatureAppearance);
            return signedData;
        }

        private SignatureAppearanceDetails CreateSignatureAppearanceDetails()
        {
            var signatureApperance = new SignatureAppearanceDetails
            {
                Contact = "",
                Reason = "",
                Location = "",
                PageNumber = 1,
                Rectangle = new Rectangle(360, 60, 100, 40)
            };

            return signatureApperance;
        }
    }
}
