using Microsoft.AspNetCore.Mvc;
using PdfAutoSigner.LocalApi.Models;
using PdfAutoSigner.LocalApi.Services;
using System.Text.Json;

namespace PdfAutoSigner.LocalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignController : ControllerBase
    {
        private ISignerService signerService;
        private ISignaturesProviderService signaturesProviderService;
        public SignController(ISignerService signerService, ISignaturesProviderService signaturesProviderService)
        {
            this.signerService = signerService;
            this.signaturesProviderService = signaturesProviderService;   
        }

        [HttpGet]
        public ActionResult<List<string>> GetAvailableSignatures()
        {
            var availableSignatures = signaturesProviderService.GetAvailableSignatures();
            var signatureNames = availableSignatures.Select(s => s.GetSignatureIdentifyingName()).ToList();

            return Ok(signatureNames);
        }

        [HttpPost]
        public IActionResult Sign([FromForm] IFormFile file, [FromForm] string inputDataJson)
        {
            var inputData = JsonSerializer.Deserialize<SignInputData>(inputDataJson);
            if (string.IsNullOrWhiteSpace(inputData?.Pin))
            {
                return BadRequest("The pin must be specified.");
            }
            if (string.IsNullOrWhiteSpace(inputData.SignatureName))
            {
                return BadRequest("A signature must be selected.");
            }

            var inputMemStream = new MemoryStream();
            file.CopyTo(inputMemStream);
            inputMemStream.Seek(0, SeekOrigin.Begin);

            var outputMemStream = signerService.Sign(inputMemStream, signatureIdentifyingName: inputData.SignatureName, pin: inputData.Pin);

            // Copy to a byte[] to return it to caller
            byte[] buffer = new byte[file.Length];
            buffer = outputMemStream.ToArray();
            outputMemStream.Close();

            return File(buffer, System.Net.Mime.MediaTypeNames.Application.Octet);
        }
    }
}
