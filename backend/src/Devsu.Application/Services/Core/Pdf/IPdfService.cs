namespace Devsu.Application.Services.Core.Pdf;

public interface IPdfService
{
    string GeneratePdfAsBase64(string html = "");
    byte[] GeneratePdfAsBytes(string? html = "");
}