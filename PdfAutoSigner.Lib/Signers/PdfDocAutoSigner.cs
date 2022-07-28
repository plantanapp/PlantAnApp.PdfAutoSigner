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

using iText.Kernel.Pdf;
using iText.Signatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PdfAutoSigner.Lib.ExtensionMethods;
using PdfAutoSigner.Lib.Signatures;
using static iText.Signatures.PdfSigner;

namespace PdfAutoSigner.Lib.Signers
{
    public class PdfDocAutoSigner : IDocAutoSigner
    {
        private readonly ILogger<PdfDocAutoSigner> logger;

        public PdfDocAutoSigner(ILogger<PdfDocAutoSigner>? logger = null)
        {
            this.logger = logger ?? NullLogger<PdfDocAutoSigner>.Instance;
        }

        public MemoryStream Sign(Stream inputStream, IExternalSignatureWithChain externalSignature, SignatureAppearanceDetails signatureAppearanceDetails)
        {
            using var reader = new PdfReader(inputStream);
            using var tempOutputStream = new MemoryStream(0);

            var stampingProperties = new StampingProperties().UseAppendMode();

            // Signer.SignDetached will close the stream. We need to create a temporary stream to be used from the signer and then copy the content into the output

            var signer = new PdfSigner(reader, tempOutputStream, stampingProperties);

            // Set appearance
            signer.UpdateSignatureAppearance(signatureAppearanceDetails);

            // TODO: Might need to give extimated size
            signer.SignDetached(externalSignature, externalSignature.GetChain(), null, null, null, 0, CryptoStandard.CMS);
            logger.LogInformation($"Signed document with signature {externalSignature.GetSignatureIdentifyingName()}");

            // Copy the data from the closed temporary output stream into the final output stream.
            var outputData = tempOutputStream.GetBuffer();
            return new MemoryStream(outputData);
        }
    }
}
