using AMC_Generator.Models;
using Microsoft.Extensions.Logging;
using QuestPDF.Drawing;
using QuestPDF.Fluent;

namespace AMC_Generator.Services;

public class AMCPDFService
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<AMCPDFService> _log;

    public AMCPDFService(IWebHostEnvironment env,
                         ILogger<AMCPDFService> log)
    {
        _env = env;
        _log = log;
    }

    /// <summary>
    /// Generates an Arabic AMC PDF, stores it under wwwroot/AMCs,
    /// and returns the relative URL ("/AMCs/xxx.pdf").
    /// Returns null if generation fails.
    /// </summary>
    public string? GeneratePdf(AMC amc, Building b, Owner o)
    {
        try
        {
            /* ---------- 1. Build target folder  (BLD‑{id}\yyyy) ---------- */
            var root = Path.Combine(_env.WebRootPath, "AMCs");
            var bFolder = $"BLD-{b.Id}";
            var year = DateTime.Now.ToString("yyyy");

            var dir = Path.Combine(root, bFolder, year);
            Directory.CreateDirectory(dir);               // makes all levels

            /* ---------- 2. Register font (optional) ---------- */
            var fontPath = Path.Combine(_env.WebRootPath, "fonts", "Tajawal-Regular.ttf");
            if (File.Exists(fontPath))
                FontManager.RegisterFont(File.OpenRead(fontPath));

            /* ---------- 3. Filename ---------- */
            var ts = DateTime.Now.ToString("yyyyMMddHHmmss");
            var fileName = $"AMC_{bFolder}_{ts}.pdf";
            var fullPath = Path.Combine(dir, fileName);

            /* ---------- 4. Compose PDF (unchanged) ---------- */
            Document.Create(c =>
            {
                c.Page(p =>
                {
                    p.Margin(25);
                    p.DefaultTextStyle(t => t.FontFamily("Times New Roman").FontSize(14));

                    p.Header().AlignRight()
                              .Text("عقد الصيانة السنوي")
                              .FontSize(20).Bold();

                    p.Content().Column(col =>
                    {
                        col.Spacing(5);
                        col.Item().AlignRight().Text($"اسم المبنى: {b.Name}");
                        col.Item().AlignRight().Text($"العنوان: {b.Address}");
                        col.Item().AlignRight().Text($"اسم المالك: {o.FullName}");
                        col.Item().AlignRight().Text($"رقم المشروع: {amc.ProjectNumber}");
                        col.Item().AlignRight().Text($"قيمة العقد: {amc.AMCValue:N2} درهم");
                        col.Item().AlignRight().Text($"تاريخ البدء: {amc.StartDate:yyyy/MM/dd}");
                        col.Item().AlignRight().Text($"تاريخ الانتهاء: {amc.EndDate:yyyy/MM/dd}");
                    });

                    p.Footer().AlignRight()
                              .Text("تم إنشاء العقد بواسطة النظام")
                              .FontSize(10).Italic();
                });
            })
            .GeneratePdf(fullPath);

            /* ---------- 5. Return URL ---------- */
            var rel = $"/AMCs/{bFolder}/{year}/{fileName}";
            return rel;
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "PDF generation FAILED for AMC {ProjectNumber}", amc.ProjectNumber);
            return null;
        }
    }
}
