namespace Devsu.Application.Services.Core.Pdf;

public class PdfService : IPdfService
{
    public string GeneratePdfAsBase64(string html = "")
    {
        var pdfBytes = GeneratePdfAsBytes(html);
        return Convert.ToBase64String(pdfBytes);
    }

    public byte[] GeneratePdfAsBytes(string? html = "")
    {
        Installation.LinuxAndDockerDependenciesAutoConfig = true;
        var renderer = new ChromePdfRenderer();
        var pdf = renderer.RenderHtmlAsPdf(html);
        using var ms = new MemoryStream();
        pdf.Stream.CopyTo(ms);
        return ms.ToArray();
    }
}   