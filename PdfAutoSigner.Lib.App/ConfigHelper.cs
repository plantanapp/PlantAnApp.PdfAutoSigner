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

using Microsoft.Extensions.Configuration;
namespace PdfAutoSigner.Lib.App
{
    internal class ConfigHelper
    {
        private const string PinSecretsManagerPath = "TokenPin";

        public static IConfigurationRoot GetIConfigurationRoot()
        {
            return new ConfigurationBuilder()
                .AddUserSecrets("fb3dd1b7-a8ac-499d-bd09-c2760b970f39")
                .Build();
        }

        public static string GetPin()
        {
            var config = GetIConfigurationRoot();
            return config[PinSecretsManagerPath];
        }
    }
}
