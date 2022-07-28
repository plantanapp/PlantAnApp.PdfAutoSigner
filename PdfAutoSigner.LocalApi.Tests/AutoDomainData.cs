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

using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace PdfAutoSigner.LocalApi.Tests
{
    public class AutoDomainDataAttribute: AutoDataAttribute
    {
        public AutoDomainDataAttribute():
            base(() => new Fixture().Customize(new AutoMoqCustomization()))
        {
        }
    }
}
