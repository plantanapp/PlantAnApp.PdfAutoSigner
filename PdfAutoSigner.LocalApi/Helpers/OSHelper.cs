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

using PdfAutoSigner.LocalApi.Config;
using System.Runtime.InteropServices;

namespace PdfAutoSigner.LocalApi.Helpers
{
    public class OSHelper : IOSHelper
    {
        public SupportedOS? GetOS()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return SupportedOS.Windows;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return SupportedOS.MacOS;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return SupportedOS.Linux;
            }

            return null;
        }

        public Architecture GetArchitecture()
        {
            return RuntimeInformation.OSArchitecture;
        }
    }
}
