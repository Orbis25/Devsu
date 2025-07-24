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
        License.LicenseKey = "IRONSUITE.ORBIS.ALONZO.UNICARIBE.EDU.DO.8326-C5C3E2FB5B-G6BW27T56NMHDS-VBLXRNHM4QF5-2T323ZHMLHIQ-UN4IVXVSIAEZ-KE2UEVSSIBIF-OEYDE2J3LDQV-RA6PKB-TGYJBEY6MDGQEA-DEPLOYMENT.TRIAL-LBWCQX.TRIAL.EXPIRES.22.AUG.2025";
        var renderer = new ChromePdfRenderer();
        var pdf = renderer.RenderHtmlAsPdf(html);
        using var ms = new MemoryStream();
        pdf.Stream.CopyTo(ms);
        return ms.ToArray();
    }
}