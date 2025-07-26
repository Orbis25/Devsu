using System.Reflection;
using Devsu.Infrastructure.Extensions;

IronPdf.License.LicenseKey = Environment.GetEnvironmentVariable("LICENSE_KEY") ?? throw new InvalidOperationException("License key is not set in environment variables.");

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
IronPdf.Installation.LinuxAndDockerDependenciesAutoConfig = false;
IronPdf.Installation.ChromeGpuMode = IronPdf.Engines.Chrome.ChromeGpuModes.Disabled;
IronPdf.Installation.Initialize();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var xml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
builder.Services.AddInfrastructure(builder.Configuration, xml);
builder.WebHost.UseUrls("http://+:8080");
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Map("/health", () => Results.Ok("Healthy"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseInfrastructure();
app.Run();