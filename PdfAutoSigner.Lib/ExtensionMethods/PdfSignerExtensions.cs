using iText.Signatures;
using PdfAutoSigner.Lib.Signers;

namespace PdfAutoSigner.Lib.ExtensionMethods
{
    public static class PdfSignerExtensions
    {
        public static void UpdateSignatureAppearance(this PdfSigner pdfSigner, SignatureAppearanceDetails signatureAppearanceDetails)
        {
            var appearance = pdfSigner.GetSignatureAppearance();
            appearance
                .SetReason(signatureAppearanceDetails.Reason)
                .SetContact(signatureAppearanceDetails.Contact)
                .SetLocation(signatureAppearanceDetails.Location)
                .SetPageNumber(signatureAppearanceDetails.PageNumber)
                .SetPageRect(signatureAppearanceDetails.Rectangle);
            appearance.SetRenderingMode(PdfSignatureAppearance.RenderingMode.NAME_AND_DESCRIPTION);
        }
    }
}
