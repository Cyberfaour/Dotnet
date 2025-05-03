using AMC_Generator.Data;
using AMC_Generator.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

/* ----------  Logging  ---------- */
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);
QuestPDF.Settings.License = LicenseType.Community;
/* ----------  EF Core  ---------- */
builder.Services.AddDbContext<AMC_GeneratorContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("AMC_GeneratorContext")
        ?? throw new InvalidOperationException("Connection string 'AMC_GeneratorContext' not found.")));

/* ----------  DI  ---------- */
builder.Services.AddScoped<AMCPDFService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

/* ----------  Pipeline  ---------- */
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

/* ----------  Routes  ---------- */
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=AMCs}/{action=Index}/{id?}");

app.Run();
