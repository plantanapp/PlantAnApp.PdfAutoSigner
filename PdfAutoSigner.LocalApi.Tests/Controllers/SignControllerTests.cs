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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PdfAutoSigner.Lib.Signatures;
using PdfAutoSigner.LocalApi.Controllers;
using PdfAutoSigner.LocalApi.Models;
using PdfAutoSigner.LocalApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PdfAutoSigner.LocalApi.Tests.Controllers
{
    public class SignControllerTests
    {
        [Theory]
        [AutoDomainData]
        public void GetAvailableSignatures_ReturnsAllSignaturesIdentifyingNames(
            Mock<IExternalSignatureWithChain> signatureMock1, Mock<IExternalSignatureWithChain> signatureMock2,
            [Frozen] Mock<ISignaturesProviderService> signaturesProviderServiceMock, [NoAutoProperties] SignController signController)
        {
            signatureMock1.Setup(s => s.GetSignatureIdentifyingName()).Returns("Sign1");
            signatureMock2.Setup(s => s.GetSignatureIdentifyingName()).Returns("Sign2");

            signaturesProviderServiceMock.Setup(sp => sp.GetAvailableSignatures()).Returns(new List<IExternalSignatureWithChain>() 
            { 
                signatureMock1.Object, signatureMock2.Object 
            });    

            var result = signController.GetAvailableSignatures();

            var okResult = result.Result as OkObjectResult;
            var signatures = okResult?.Value as List<string>;
            Assert.Equal(2, signatures?.Count);
            Assert.Contains("Sign1", signatures);
            Assert.Contains("Sign2", signatures);
        }

        [Theory]
        [AutoDomainData]
        public void Sign_MissingPin_ReturnsBadRequest(
            [Frozen] Mock<IFormFile> formFileMock, 
            [NoAutoProperties] SignController signController)
        {
            var inputData = new SignInputData { Pin = "" };
            var inputDataJson = JsonSerializer.Serialize(inputData);

            var result = signController.Sign(formFileMock.Object, inputDataJson);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [AutoDomainData]
        public void Sign_MissingSignature_ReturnsBadRequest(
            [Frozen] Mock<IFormFile> formFileMock,
            [NoAutoProperties] SignController signController)
        {
            var inputData = new SignInputData { SignatureName = "" };
            var inputDataJson = JsonSerializer.Serialize(inputData);

            var result = signController.Sign(formFileMock.Object, inputDataJson);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [AutoDomainData]
        public void Sign_CallsSignWithCorrectParameters(
            [Frozen] Mock<IFormFile> formFileMock, [Frozen] Mock<ISignerService> signerServiceMock,
            [NoAutoProperties] SignController signController)
        {
            var inputData = new SignInputData { Pin = "123456", SignatureName = "Sign1" };
            var inputDataJson = JsonSerializer.Serialize(inputData);

            Stream? inputStream = null;
            formFileMock.Setup(f => f.CopyTo(It.IsAny<Stream>())).Callback<Stream>(s => inputStream = s);

            var result = signController.Sign(formFileMock.Object, inputDataJson);

            signerServiceMock.Verify(s => s.Sign(inputStream!, "Sign1", "123456"));
        }

        [Theory]
        [AutoDomainData]
        public void Sign_ReturnsResultFromSign(
            SignInputData signInputData, Mock<MemoryStream> outputMemStreamMock,
            [Frozen] Mock<IFormFile> formFileMock, [Frozen] Mock<ISignerService> signerServiceMock,
            [NoAutoProperties] SignController signController)
        {
            var inputDataJson = JsonSerializer.Serialize(signInputData);
            byte[] signedByteArr = Encoding.ASCII.GetBytes("test result");
            outputMemStreamMock.Setup(ms => ms.ToArray()).Returns(signedByteArr);
            signerServiceMock.Setup(s => s.Sign(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>())).Returns(outputMemStreamMock.Object);
            
            var result = signController.Sign(formFileMock.Object, inputDataJson);
            
            var fileContentResult = result as FileContentResult;
            Assert.Equal(signedByteArr, fileContentResult!.FileContents);
            Assert.Equal(System.Net.Mime.MediaTypeNames.Application.Octet, fileContentResult.ContentType);
        }
    }
}
