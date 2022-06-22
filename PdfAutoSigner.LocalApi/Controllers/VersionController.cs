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
