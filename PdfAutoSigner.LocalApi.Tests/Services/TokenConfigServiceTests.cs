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
using AutoFixture.Xunit2;
using Microsoft.Extensions.Options;
using Moq;
using PdfAutoSigner.LocalApi.Config;
using PdfAutoSigner.LocalApi.Helpers;
using PdfAutoSigner.LocalApi.Services;
using System.Runtime.InteropServices;

namespace PdfAutoSigner.LocalApi.Tests.Services
{
    public class TokenConfigServiceTests
    {
        [Theory]
        [AutoDomainData]
        public void GetPkcs11LibPathsByOS_UnsupportedOS_ThrowException(
            [Frozen] Mock<IOptionsSnapshot<TokenOptions>> tokenOptionsSnapshotMock,
            [Frozen] Mock<IOSHelper> osHelperMock, TokenConfigService tokenConfigService, Fixture fixture)
        {
            tokenOptionsSnapshotMock.Setup(to => to.Value).Returns(fixture.Create<TokenOptions>());
            osHelperMock.Setup(os => os.GetOS()).Returns<SupportedOS>(null);

            Action act = () => tokenConfigService.GetPkcs11LibPathsByOS();

            Assert.Throws<SystemException>(act);
        }

        [Theory]
        [AutoDomainData]
        public void GetPkcs11LibPathsByOS_ReturnOnlyEntriesWithMatchingOSAndArchitecture(
            [Frozen] Mock<IOptionsSnapshot<TokenOptions>> tokenOptionsSnapshotMock,
            [Frozen] Mock<IOSHelper> osHelperMock, TokenConfigService tokenConfigService, Fixture fixture)
        {
            var pkcs11LibData = fixture.CreateMany<Pkcs11LibPathData>(20);
            // Eliminate all the Win x64 combinations so that we know exactly how many we will have
            var filteredPkcs11LibData = pkcs11LibData.Where(pld => pld.OS != SupportedOS.Windows && pld.Architecture != Architecture.X64);
            // Add fixed number of win x64 entries
            var win64Pkcs11LibData = fixture.Build<Pkcs11LibPathData>().With(pld => pld.OS, SupportedOS.Windows)
                .With(pld => pld.Architecture, Architecture.X64).CreateMany<Pkcs11LibPathData>(3);
            var finalPkcs11LibData = filteredPkcs11LibData.Concat(win64Pkcs11LibData);

            var tokenOptions = new TokenOptions
            {
                Pkcs11Devices = new List<Pkcs11DeviceData>
                {
                    new Pkcs11DeviceData { Name = "Device1", Pkcs11LibPaths = finalPkcs11LibData.ToList() }
                }
            };
            tokenOptionsSnapshotMock.Setup(to => to.Value).Returns(tokenOptions);
            osHelperMock.Setup(os => os.GetOS()).Returns(SupportedOS.Windows);
            osHelperMock.Setup(os => os.GetArchitecture()).Returns(Architecture.X64);

            var libPaths = tokenConfigService.GetPkcs11LibPathsByOS();

            Assert.Equal(3, libPaths.Count);
            var expectedPaths = win64Pkcs11LibData.Select(pld => pld.LibPath);
            Assert.Equal(expectedPaths.OrderBy(p => p), libPaths.OrderBy(p => p));
        }

        [Theory]
        [AutoDomainData]
        public void GetIssuerNames_ReturnCertIssuerNames(
            [Frozen] Mock<IOptionsSnapshot<TokenOptions>> tokenOptionsSnapshotMock,
            TokenConfigService tokenConfigService, Fixture fixture)
        {
            var certData1 = fixture.Build<CertificateData>().With(c => c.CertificateIssuerName, "Issuer1").Create();
            var certData2 = fixture.Build<CertificateData>().With(c => c.CertificateIssuerName, "Issuer2").Create();
            var certIssuerData = new List<CertificateData> { certData1, certData2 };
            var tokenOptions = new TokenOptions
            {
                Certificates = certIssuerData
            };
            tokenOptionsSnapshotMock.Setup(to => to.Value).Returns(tokenOptions);

            var certIssuerNames = tokenConfigService.GetIssuerNames();

            Assert.Equal(2, certIssuerNames.Count);
            Assert.Contains("Issuer1", certIssuerNames);
            Assert.Contains("Issuer2", certIssuerNames);
        }
    }
}
