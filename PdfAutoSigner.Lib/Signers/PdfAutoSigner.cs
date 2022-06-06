using iText.Kernel.Pdf;
using iText.Signatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PdfAutoSigner.Lib.ExtensionMethods;
using PdfAutoSigner.Lib.Signatures;
using static iText.Signatures.PdfSigner;

namespace PdfAutoSigner.Lib.Signers
{
    public class PdfAutoSigner : IPdfAutoSigner
    {
        private readonly ILogger<PdfAutoSigner> logger;

        public PdfAutoSigner(ILogger<PdfAutoSigner>? logger = null)
        {
            if (logger == null)
            {
                this.logger = NullLogger<PdfAutoSigner>.Instance;
            }
        }

        public MemoryStream Sign(Stream inputStream, IExternalSignatureWithChain externalSignature, SignatureAppearanceDetails signatureAppearanceDetails)
        {
            using (PdfReader reader = new PdfReader(inputStream))
            {
                var stampingProperties = new StampingProperties().UseAppendMode();

                // Signer.SignDetached will close the stream. We need to create a temporary stream to be used from the signer and then copy the content into the output
                var tempOutputStream = new MemoryStream(0);
                var signer = new PdfSigner(reader, tempOutputStream, stampingProperties);

                // Set appearance
                signer.UpdateSignatureAppearance(signatureAppearanceDetails);

                // TODO: Might need to give extimated size
                signer.SignDetached(externalSignature, externalSignature.GetChain(), null, null, null, 0, CryptoStandard.CMS);

                // Copy the data from the closed temporary output stream into the final output stream.
                var outputData = tempOutputStream.GetBuffer();
                return new MemoryStream(outputData);
            }
        }
    }
}
