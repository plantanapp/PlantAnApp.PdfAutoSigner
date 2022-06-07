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
        public SignController(ISignerService signerService)
        {
            this.signerService = signerService;
        }

        [HttpGet]
        public ActionResult<List<string>> GetAvailableSignatures()
        {
            // TODO: Handle exception cases
            var availableSignatures = signerService.GetAvailableSignatures();
            var signatureNames = availableSignatures.Select(s => s.GetSignatureIdentifyingName()).ToList();

            return Ok(signatureNames);
        }

        [HttpPost]
        public IActionResult Post([FromForm] IFormFile file, [FromForm] string inputDataJson)
        {
            var inputData = JsonSerializer.Deserialize<SignInputData>(inputDataJson);
            var contentStream = new MemoryStream();
            file.CopyTo(contentStream);

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
