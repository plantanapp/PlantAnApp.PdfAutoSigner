// Needed to be able to call UseWindowsServices
using Microsoft.Extensions.Hosting.WindowsServices;

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

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
