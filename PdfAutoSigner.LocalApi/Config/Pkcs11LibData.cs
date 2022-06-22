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

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.InteropServices;

namespace PdfAutoSigner.LocalApi.Config
{
    public class Pkcs11LibPathData
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public SupportedOS OS { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Architecture Architecture { get; set; }

        public string? LibPath { get; set; }
    }
}
