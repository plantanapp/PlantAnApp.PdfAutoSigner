using iText.Kernel.Geom;

namespace PdfAutoSigner.Lib.Signers
{
    public class SignatureAppearanceDetails
    {
        public string Reason { get; set; }
        public string Contact { get; set; }
        public string Location { get; set; }
        public int PageNumber { get; set; }
        public Rectangle Rectangle { get; set; }
    }
}
