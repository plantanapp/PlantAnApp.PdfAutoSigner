// See https://aka.ms/new-console-template for more information
using CommandLine;
using iText.Kernel.Geom;
using Microsoft.Extensions.Logging;
using PdfAutoSigner.Lib.App;
using PdfAutoSigner.Lib.Signatures;
using PdfAutoSigner.Lib.Signers;

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddDebug().AddConsole();
});
ILogger logger = loggerFactory.CreateLogger<Program>();
// Read input params from arguments
var result = Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed(RunOptions)
    .WithNotParsed(HandleParseErrors)
    ;

void RunOptions(CommandLineOptions options)
{
    var pin = !string.IsNullOrEmpty(options.Pin) ? options.Pin : ConfigHelper.GetPin();

    if (string.IsNullOrEmpty(pin))
    {
        logger.LogCritical("You must specify the pin");
        throw new InvalidProgramException("You must specify the pin");
    }

    // Create the signature
    var factory = new SignatureFactory();
    IExternalSignatureWithChain? signature = null;
    if (options.UseCertificates)
    {
        var availableSignatures = factory.GetAvailableX509Certificate2Signatures(new List<string> { options.LibPathOrIssuerName });
        signature = availableSignatures?.ElementAt((int)options.SignatureIdx);
    }
    else
    {
        var availableSignatures = factory.GetAvailablePkcs11Signatures(new List<string> { options.LibPathOrIssuerName });
        signature = availableSignatures?.ElementAt((int)options.SignatureIdx);
    }

    signature = signature?.Select(pin);
    if (signature == null)
    {
        throw new ApplicationException("Could not get the correct signature");
    }

    IDocAutoSigner docSigner = new PdfDocAutoSigner();
    // Read file
    using (var inputStream = new MemoryStream())
    {
        using (var inputFileStream = new FileStream(options.InputFilePath, FileMode.Open, FileAccess.Read))
        {
            inputFileStream.CopyTo(inputStream);
        }

        // Need to move to the beginning of the stream
        inputStream.Seek(0, SeekOrigin.Begin);

        var signatureApperance = new SignatureAppearanceDetails
        {
            Contact = "CustomContact",
            Reason = "CustomReason",
            Location = "Bucharest",
            PageNumber = 1,
            Rectangle = new Rectangle(360, 60, 100, 40)
        };

        // Sign stream
        using (var outputStream = docSigner.Sign(inputStream, signature, signatureApperance))
        {

            // Write file
            using (FileStream outputFileStream = new FileStream(options.OutputFilePath, FileMode.Create, System.IO.FileAccess.Write))
            {
                outputStream.CopyTo(outputFileStream);
            }
        }
    }
}

void HandleParseErrors(IEnumerable<Error> errors)
{
    throw new InvalidProgramException("Could not parse command line arguments.");
}