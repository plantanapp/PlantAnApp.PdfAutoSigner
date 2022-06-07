// Needed to be able to call UseWindowsServices
using Microsoft.Extensions.Hosting.WindowsServices;
using PdfAutoSigner.Lib.Signatures;
using PdfAutoSigner.Lib.Signers;
using PdfAutoSigner.LocalApi.Helpers;
using PdfAutoSigner.LocalApi.Services;

var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
};
var builder = WebApplication.CreateBuilder(options);

// Add support to be able to run as Windows service or Linux \ Mac daemon
builder.Host.UseWindowsService().UseSystemd();

// Add configuration settings
builder.Configuration.AddJsonFile("hostsettings.json", optional: true);
builder.Configuration.AddJsonFile("tokensettings.json", optional: true);

// Enable CORS
var allowedOrigins = builder.Configuration["AllowedOrigins"];
if (!String.IsNullOrEmpty(allowedOrigins))
{
    var origins = allowedOrigins.Split(";");
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins(origins).AllowAnyMethod().AllowAnyHeader();
        });
    });
}

// Add services to the container.
builder.Services.AddTransient<IOSHelper, OSHelper>();
builder.Services.AddTransient<ITokenConfigService, TokenConfigService>();
builder.Services.AddTransient<ISignatureFactory, SignatureFactory>();
builder.Services.AddTransient<IDocAutoSigner, PdfDocAutoSigner>();
builder.Services.AddTransient<ISignerService, SignerService>();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.Run();
