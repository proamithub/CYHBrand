using CYHBrandData;
using CYHBrandData.Dapper;
using CYHBrandModel;
using CYHBrandRepo;
using CYHBrandRepo.Utilities;
using CYHBrandUI.Helpers;
using CYHBrandUI.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json.Serialization;

// ── Serilog early init ────────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/cyhbrand-.log", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((ctx, lc) => lc
        .ReadFrom.Configuration(ctx.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/cyhbrand-.log", rollingInterval: RollingInterval.Day));

    // ── Config helpers ────────────────────────────────────────────────────────
    WebConfigSetting.Initialize(builder.Configuration);

    // ── Dapper ───────────────────────────────────────────────────────────────
    builder.Services.AddTransient<IDapperContext>(_ =>
        new DapperContext(builder.Configuration.GetConnectionString("DefaultConnection")!));

    // ── UnitOfWork ───────────────────────────────────────────────────────────
    builder.Services.AddHttpClient();
    builder.Services.AddTransient<IMailClient, MailClient>();
    builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<IAiBannerService, AiBannerService>();
    builder.Services.AddSingleton<IUrlEncryptor, UrlEncryptor>();   // URL ID encryption

    // ── HttpContext / Session ─────────────────────────────────────────────────
    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(o =>
    {
        o.IdleTimeout        = TimeSpan.FromMinutes(30);
        o.Cookie.HttpOnly    = true;
        o.Cookie.IsEssential = true;
    });

    // ── File upload limits ────────────────────────────────────────────────────
    builder.Services.Configure<FormOptions>(o =>
    {
        o.ValueLengthLimit        = 10 * 1024 * 1024;   // 10 MB
        o.MultipartBodyLengthLimit = 128 * 1024 * 1024; // 128 MB
    });

    // ── Cookie Auth ───────────────────────────────────────────────────────────
    builder.Services.Configure<CookiePolicyOptions>(o =>
    {
        o.CheckConsentNeeded    = _ => false;
        o.MinimumSameSitePolicy = SameSiteMode.None;
    });
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(o =>
        {
            o.Cookie.Name       = "CYHBrandApp";
            o.LoginPath         = "/Account/Login";
            o.AccessDeniedPath  = "/Account/AccessDenied";
            o.SlidingExpiration = true;
            o.Cookie.HttpOnly   = true;
            o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            o.Cookie.SameSite   = SameSiteMode.Lax;
            o.ExpireTimeSpan    = TimeSpan.FromHours(8);
        });

    // ── Data Protection ───────────────────────────────────────────────────────
    var keysDir = new DirectoryInfo(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "keys"));
    keysDir.Create();
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(keysDir)
        .SetApplicationName("CYHBrand")
        .SetDefaultKeyLifetime(TimeSpan.FromDays(30));

    // ── EF Core ───────────────────────────────────────────────────────────────
    builder.Services.AddDbContext<DataContext>(o =>
        o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // ── MVC ───────────────────────────────────────────────────────────────────
    var mvc = builder.Services.AddControllersWithViews()
        .AddJsonOptions(o =>
        {
            o.JsonSerializerOptions.PropertyNamingPolicy = null;
            o.JsonSerializerOptions.ReferenceHandler     = ReferenceHandler.IgnoreCycles;
        })
        .AddNewtonsoftJson();

    // ── In-Memory Cache (templates, categories, dashboard stats) ──────────────
    builder.Services.AddMemoryCache();

    // ── Response Compression ──────────────────────────────────────────────────
    builder.Services.AddResponseCompression(o => { o.EnableForHttps = true; });

#if DEBUG
    if (builder.Environment.IsDevelopment())
        mvc.AddRazorRuntimeCompilation();
#endif

    // ─────────────────────────────────────────────────────────────────────────
    var app = builder.Build();
    // ─────────────────────────────────────────────────────────────────────────

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseResponseCompression();
    app.UseHttpsRedirection();

    // Static files — aggressive browser caching (1 year for lib assets)
    app.UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = ctx =>
        {
            var path = ctx.File.PhysicalPath ?? "";
            // Cache lib/ assets for 1 year, others for 1 day
            var maxAge = path.Contains("\\lib\\") ? 31536000 : 86400;
            ctx.Context.Response.Headers.Append("Cache-Control", $"public,max-age={maxAge},immutable");
        }
    });
    app.UseSerilogRequestLogging();
    app.UseSession();
    app.UseCookiePolicy();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute("areas",   "{area:exists}/{controller=Home}/{action=Index}/{id?}");
    app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}
