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

namespace PdfAutoSigner.Lib.Signers
{
    public class SignatureAppearanceDetails
    {
        public string? Reason { get; set; }
        public string? Contact { get; set; }
        public string? Location { get; set; }
        public int PageNumber { get; set; }
        public Rectangle? Rectangle { get; set; }
    }
}
