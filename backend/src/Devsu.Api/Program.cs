using System.Reflection;
using Devsu.Infrastructure.Extensions;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
IronPdf.Installation.LinuxAndDockerDependenciesAutoConfig = false;
IronPdf.Installation.ChromeGpuMode = IronPdf.Engines.Chrome.ChromeGpuModes.Disabled;
IronPdf.Installation.Initialize();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var xml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
builder.Services.AddInfrastructure(builder.Configuration, xml);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseInfrastructure();
app.Run();