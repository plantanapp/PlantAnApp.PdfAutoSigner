using AutoFixture;
using AutoFixture.Xunit2;
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
    }
}
