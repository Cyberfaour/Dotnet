using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMC_Generator.Data;
using AMC_Generator.Models;
using AMC_Generator.Services;
using Microsoft.Extensions.Logging;

namespace AMC_Generator.Controllers;

/// <summary>
/// Full CRUD controller for Annual‑Maintenance‑Contract (AMC) entities.
/// Generates / regenerates a PDF via <see cref="AMCPDFService"/> and
/// stores it under <c>wwwroot/AMCs/BLD‑{BuildingId}/{yyyy}/…</c>.
/// </summary>
public class AMCsController : Controller
{
    private readonly AMC_GeneratorContext _ctx;
    private readonly AMCPDFService _pdf;
    private readonly ILogger<AMCsController> _log;
    private readonly IWebHostEnvironment _env;

    public AMCsController(AMC_GeneratorContext ctx,
                          AMCPDFService pdf,
                          ILogger<AMCsController> log,
                          IWebHostEnvironment env)
        => (_ctx, _pdf, _log, _env) = (ctx, pdf, log, env);

    /* ─────────────────────────── LIST ─────────────────────────── */

    public async Task<IActionResult> Index(string status = "All", string q = null)
    {
        if (TempData["PdfErr"] is string e) ViewBag.PdfErr = e;
        if (TempData["PdfInfo"] is string i) ViewBag.PdfInfo = i;

        ViewBag.CurrentStatus = status;
        ViewBag.Search = q;

        /* 1 — Search (SQL) */
        var query = _ctx.AMC
                        .Include(a => a.Building).ThenInclude(b => b.Owner)
                        .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(a => a.ProjectNumber.Contains(q) ||
                                     a.Building.Name.Contains(q));

        /* 2 — Materialise, then status‑filter (client) */
        var amcs = (await query.ToListAsync()).AsEnumerable();

        amcs = status switch
        {
            "Active" => amcs.Where(a => a.Status == AmcStatus.Active),
            "Expiring" => amcs.Where(a => a.Status == AmcStatus.Expiring),
            "Expired" => amcs.Where(a => a.Status == AmcStatus.Expired),
            "Terminated" => amcs.Where(a => a.Status == AmcStatus.Terminated),
            _ => amcs
        };

        return View(amcs.OrderByDescending(a => a.Id));
    }

    /* ─────────────────────────── DETAILS ─────────────────────────── */

    public async Task<IActionResult> Details(int? id, bool regen = false)
    {
        if (id is null) return NotFound();

        var amc = await _ctx.AMC.Include(a => a.Building).ThenInclude(b => b.Owner)
                                .FirstOrDefaultAsync(a => a.Id == id);

        if (amc is null) return NotFound();

        if (regen || string.IsNullOrWhiteSpace(amc.PdfPath))
            await RegeneratePdfAsync(amc);

        return View(amc);
    }

    /* ─────────────────────────── CREATE ─────────────────────────── */

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Buildings = _ctx.Building.Include(b => b.Owner).ToList();
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("AMCValue,StartDate,EndDate,BuildingId")] AMC amc)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Buildings = _ctx.Building.ToList();
            return View(amc);
        }

        var b = await _ctx.Building.FindAsync(amc.BuildingId);
        amc.ProjectNumber = $"BLD-{b!.Id}-{DateTime.Now:yyyyMMdd}";

        _ctx.Add(amc);
        await _ctx.SaveChangesAsync();

        await RegeneratePdfAsync(amc);
        return RedirectToAction(nameof(Index));
    }

    /* ─────────────────────────── EDIT ─────────────────────────── */

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();

        var amc = await _ctx.AMC.FindAsync(id);
        if (amc is null) return NotFound();

        ViewBag.Buildings = _ctx.Building.ToList();
        return View(amc);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("Id,AMCValue,StartDate,EndDate,BuildingId,TerminatedOn")] AMC amc)
    {
        if (id != amc.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            ViewBag.Buildings = _ctx.Building.ToList();
            return View(amc);
        }

        _ctx.Update(amc);
        await _ctx.SaveChangesAsync();

        await RegeneratePdfAsync(amc);
        return RedirectToAction(nameof(Index));
    }

    /* ─────────────────────────── DELETE ─────────────────────────── */

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();

        var amc = await _ctx.AMC
            .Include(a => a.Building).ThenInclude(b => b.Owner)
            .FirstOrDefaultAsync(a => a.Id == id);

        return amc is null ? NotFound() : View(amc);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var amc = await _ctx.AMC.FindAsync(id);
        if (amc != null)
        {
            /* delete file */
            if (!string.IsNullOrWhiteSpace(amc.PdfPath))
            {
                var full = Path.Combine(_env.WebRootPath, amc.PdfPath.TrimStart('/'));
                if (System.IO.File.Exists(full)) System.IO.File.Delete(full);
            }

            _ctx.AMC.Remove(amc);
            await _ctx.SaveChangesAsync();
            TempData["PdfInfo"] = "Contract deleted.";
        }
        return RedirectToAction(nameof(Index));
    }

    /* ─────────────────────────── PDF helper ─────────────────────────── */

    private async Task RegeneratePdfAsync(AMC amc)
    {
        var full = await _ctx.AMC.Where(a => a.Id == amc.Id)
                      .Include(a => a.Building).ThenInclude(b => b.Owner)
                      .SingleAsync();

        var newPath = _pdf.GeneratePdf(full, full.Building, full.Building.Owner);

        if (string.IsNullOrWhiteSpace(newPath))
            TempData["PdfErr"] = "PDF generation failed.";
        else
        {
            full.PdfPath = newPath;
            _ctx.Update(full);
            await _ctx.SaveChangesAsync();
            TempData["PdfInfo"] = "PDF generated successfully.";
        }
    }
}
