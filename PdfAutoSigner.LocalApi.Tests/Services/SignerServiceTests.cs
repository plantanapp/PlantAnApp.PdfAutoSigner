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
using iText.Kernel.Pdf;
using iText.Signatures;
using Moq;
using PdfAutoSigner.Lib.Signatures;
using PdfAutoSigner.Lib.Signers;
using PdfAutoSigner.LocalApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfAutoSigner.LocalApi.Tests.Services
{
    public class SignerServiceTests
    {
        private const string SignatureIdentifyingName = "NewSignature";
        private const string Pin = "123456";

        [Theory]
        [AutoDomainData]
        public void Sign_NoSignatureWithName_ThrowException(
            Mock<IExternalSignatureWithChain> signatureMock,
            [Frozen] Mock<ISignaturesProviderService> signaturesProviderServiceMock,
            SignerService signerService, Fixture fixture)
        {
            signatureMock.Setup(s => s.GetSignatureIdentifyingName()).Returns("Inexistent");
            signaturesProviderServiceMock.Setup(sp => sp.GetAvailableSignatures()).Returns(new List<IExternalSignatureWithChain> { signatureMock.Object });

            Action act = () => signerService.Sign(fixture.Create<Stream>(), SignatureIdentifyingName, Pin);

            Assert.Throws<ApplicationException>(act);
        }

        [Theory]
        [AutoDomainData]
        public void Sign_SelectUsesPin(
            Mock<IExternalSignatureWithChain> signatureMock,
            [Frozen] Mock<ISignaturesProviderService> signaturesProviderServiceMock,
            SignerService signerService, Fixture fixture)
        {
            signatureMock.Setup(s => s.GetSignatureIdentifyingName()).Returns("NewSignature");
            signaturesProviderServiceMock.Setup(sp => sp.GetAvailableSignatures()).Returns(new List<IExternalSignatureWithChain> { signatureMock.Object });

            signerService.Sign(fixture.Create<Stream>(), SignatureIdentifyingName, Pin);

            signatureMock.Verify(s => s.Select(Pin), Times.Once());
        }

        [Theory]
        [AutoDomainData]
        public void Sign_SelectThrowsException_ThrowException(
            Mock<IExternalSignatureWithChain> signatureMock,
            [Frozen] Mock<ISignaturesProviderService> signaturesProviderServiceMock,
            SignerService signerService, Fixture fixture)
        {
            signatureMock.Setup(s => s.GetSignatureIdentifyingName()).Returns(SignatureIdentifyingName);
            signatureMock.Setup(s => s.Select(It.IsAny<string>())).Throws<SystemException>();
            signaturesProviderServiceMock.Setup(sp => sp.GetAvailableSignatures()).Returns(new List<IExternalSignatureWithChain> { signatureMock.Object });

            Action act = () => signerService.Sign(fixture.Create<Stream>(), SignatureIdentifyingName, Pin);

            Assert.Throws<ApplicationException>(act);
        }

        [Theory]
        [AutoDomainData]
        public void Sign_UsesCorrectParameters(
            Mock<IExternalSignatureWithChain> signatureMock,
            [Frozen] Mock<ISignaturesProviderService> signaturesProviderServiceMock,
            [Frozen] Mock<IDocAutoSigner> signerMock,
            SignerService signerService, Fixture fixture)
        {
            var inputStream = fixture.Create<Stream>();
            signatureMock.Setup(s => s.GetSignatureIdentifyingName()).Returns(SignatureIdentifyingName);
            signatureMock.Setup(s => s.Select(Pin)).Returns(signatureMock.Object);
            signaturesProviderServiceMock.Setup(sp => sp.GetAvailableSignatures()).Returns(new List<IExternalSignatureWithChain> { signatureMock.Object });

            signerService.Sign(inputStream, SignatureIdentifyingName, Pin);

            signerMock.Verify(s => s.Sign(inputStream, signatureMock.Object,  It.IsAny<SignatureAppearanceDetails>()), Times.Once());
        }

        [Theory]
        [AutoDomainData]
        public void Sign_SignActualPdf_ReturnsSignedStreamWithOneSignature([Frozen] Mock<ISignaturesProviderService> signaturesProviderServiceMock,
            //SignerService signerService, 
            Fixture fixture)
        {
            string? password = null;
            var cert = X509CertificateFactory.CreateRsaCertificate(password);
            var certSignatures = new List<IExternalSignatureWithChain> { new X509Certificate2Signature(cert) };
            var signatureName = certSignatures[0].GetSignatureIdentifyingName();
            signaturesProviderServiceMock.Setup(p => p.GetAvailableSignatures()).Returns(certSignatures);
            var docService = new PdfDocAutoSigner();
            var signerService = new SignerService(signaturesProviderServiceMock.Object, docService);

            using MemoryStream inputStream = new MemoryStream();
            ReadFile("hello.pdf", inputStream);

            var outputStream = signerService.Sign(inputStream, signatureName, password);

            using var reader = new PdfReader(outputStream);
            using var doc = new PdfDocument(reader);
            var signUtil = new SignatureUtil(doc);
            var names = signUtil.GetSignatureNames();
            
            Assert.Equal(1, names.Count);
        }

        private void ReadFile(string fileName, MemoryStream ms)
        {
            string path = Path.Combine(Environment.CurrentDirectory, @"Files", fileName);
            using (var file = new FileStream(path, FileMode.Open, FileAccess.Read))
            file.CopyTo(ms);
            ms.Seek(0, SeekOrigin.Begin);
        }
    }
}
