using AutoFixture;
using AutoFixture.Xunit2;
using Moq;
using Net.Pkcs11Interop.HighLevelAPI;
using PdfAutoSigner.Lib.Signatures;
using PdfAutoSigner.LocalApi.Services;

namespace PdfAutoSigner.LocalApi.Tests.Services
{
    public class SignaturesProviderServiceTests
    {
        [Theory]
        [AutoDomainData]
        public void GetAvailableSignatures_ReturnsPkcs11AndCertSignatures([Frozen] Mock<ISignatureFactory> signatureFactoryMock, 
            SignaturesProviderService signaturesProviderService, Fixture fixture)
        {
            var iSlot = fixture.Create<ISlot>();
            var pkcs11Signatures = Enumerable.Repeat(1, 5).Select(_ => new Pkcs11Signature(iSlot)).ToList();
            var cert = X509CertificateFactory.CreateRsaCertificate("P@55w0rd");
            var certSignatures = Enumerable.Repeat(1, 3).Select(_ => new X509Certificate2Signature(cert)).ToList();
            signatureFactoryMock.Setup(f => f.GetAvailablePkcs11Signatures(It.IsAny<List<string>>())).Returns(pkcs11Signatures);
            signatureFactoryMock.Setup(f => f.GetAvailableX509Certificate2Signatures(It.IsAny<List<string>>())).Returns(certSignatures);

            var signatures = signaturesProviderService.GetAvailableSignatures();

            Assert.Equal(8, signatures.Count);
            Assert.Contains(signatures, x => pkcs11Signatures.Contains(x));
            Assert.Contains(signatures, x => certSignatures.Contains(x));
        }

        [Theory]
        [AutoDomainData]
        public void GetAvailableSignatures_UsesConfigOptions([Frozen] Mock<ISignatureFactory> signatureFactoryMock,
            [Frozen] Mock<ITokenConfigService> tokenConfigServiceMock, SignaturesProviderService signaturesProviderService, Fixture fixture)
        {
            var pkcsLibPaths = fixture.CreateMany<string>(5).ToList();
            var certIssuerNames = fixture.CreateMany<string>(5).ToList();
            tokenConfigServiceMock.Setup(t => t.GetPkcs11LibPathsByOS()).Returns(pkcsLibPaths);
            tokenConfigServiceMock.Setup(t => t.GetIssuerNames()).Returns(certIssuerNames);

            var signatures = signaturesProviderService.GetAvailableSignatures();

            signatureFactoryMock.Verify(f => f.GetAvailablePkcs11Signatures(pkcsLibPaths), Times.Once());
            signatureFactoryMock.Verify(f => f.GetAvailableX509Certificate2Signatures(certIssuerNames), Times.Once());
        }
    }
}
