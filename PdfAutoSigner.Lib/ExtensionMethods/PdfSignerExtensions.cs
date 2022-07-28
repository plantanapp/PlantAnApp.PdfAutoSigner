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
using PdfAutoSigner.Lib.Signers;

namespace PdfAutoSigner.Lib.ExtensionMethods
{
    public static class PdfSignerExtensions
    {
        public static void UpdateSignatureAppearance(this PdfSigner pdfSigner, SignatureAppearanceDetails signatureAppearanceDetails)
        {
            var appearance = pdfSigner.GetSignatureAppearance();
            appearance
                .SetReason(signatureAppearanceDetails.Reason)
                .SetContact(signatureAppearanceDetails.Contact)
                .SetLocation(signatureAppearanceDetails.Location)
                .SetPageNumber(signatureAppearanceDetails.PageNumber)
                .SetPageRect(signatureAppearanceDetails.Rectangle);
            appearance.SetRenderingMode(PdfSignatureAppearance.RenderingMode.NAME_AND_DESCRIPTION);
        }
    }
}
