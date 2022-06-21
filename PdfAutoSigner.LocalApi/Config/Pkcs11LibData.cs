using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.InteropServices;

namespace PdfAutoSigner.LocalApi.Config
{
    public class Pkcs11LibPathData
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public SupportedOS OS { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Architecture Architecture { get; set; }

        public string? LibPath { get; set; }
    }
}
