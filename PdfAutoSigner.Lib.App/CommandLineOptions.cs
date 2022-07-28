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

using CommandLine;
using System.Diagnostics.CodeAnalysis;

namespace PdfAutoSigner.Lib.App
{
    internal class CommandLineOptions
    {
        [Value(index: 0, Required = true, HelpText = "Input Pdf to sign.")]
        [NotNull]
        public string? InputFilePath { get; set; }

        [Value(index: 1, Required = true, HelpText = "Output Pdf to sign.")]
        [NotNull]
        public string? OutputFilePath { get; set; }

        [Value(index: 2, Required = true, HelpText = "Lib path (for PKCS11 mode) or issuer name (for certificates mode).")]
        [NotNull]
        public string? LibPathOrIssuerName { get; set; }

        [Option(shortName: 'c', Required = false, HelpText = "Use certificates to sign. By default it uses PKCS11.", Default = false)]
        public bool UseCertificates { get; set; }

        [Option(shortName: 'p', Required = false, HelpText = "Pin for the usb token. If not provided, it will try to read the 'TokenPin' from the config.", Default = null)]
        public string? Pin { get; set; }

        [Option(shortName: 'i', Required = false, HelpText = "Index of the signature to sign the certificate with.", Default = 0UL)]
        public ulong SignatureIdx { get; set; }
    }
}
