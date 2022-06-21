using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace PdfAutoSigner.LocalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        [HttpGet("app-file")]
        public ActionResult<List<string>> GetAssemblyFileVersion()
        {
            var assemblyFileVersion = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;

            return Ok(assemblyFileVersion);
        }

        /// <summary>
        /// This is what is used by the Wix Installer and shown in Windows.
        /// </summary>
        /// <returns></returns>
        [HttpGet("app-assembly")]
        public ActionResult<List<string>> GetAssemblyVersion()
        {
            var assemblyVersion = Assembly.GetEntryAssembly()?.GetName().Version;

            return Ok(assemblyVersion);
        }
    }
}
