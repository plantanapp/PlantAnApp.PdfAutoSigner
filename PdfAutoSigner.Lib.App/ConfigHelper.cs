using Microsoft.Extensions.Configuration;
namespace PdfAutoSigner.Lib.App
{
    internal class ConfigHelper
    {
        private const string PinSecretsManagerPath = "TokenPin";

        public static IConfigurationRoot GetIConfigurationRoot()
        {
            return new ConfigurationBuilder()
                .AddUserSecrets("fb3dd1b7-a8ac-499d-bd09-c2760b970f39")
                .Build();
        }

        public static string GetPin()
        {
            var config = GetIConfigurationRoot();
            return config[PinSecretsManagerPath];
        }
    }
}
