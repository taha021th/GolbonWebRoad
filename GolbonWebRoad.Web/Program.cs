using GolbonWebRoad.Application;
using GolbonWebRoad.Infrastructure;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

// 1. پیکربندی لاگر اولیه (Bootstrap Logger)
// این لاگر برای ثبت وقایع قبل از ساخته شدن کامل برنامه استفاده می‌شود
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up application");


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("GolbonWebRoadDbContextConnection") ?? throw new InvalidOperationException("Connection string 'GolbonWebRoadDbContextConnection' not found.");




builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout=TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly=true;
    options.Cookie.IsEssential=true;

}
);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<GolbonWebRoad.Web.Mapping.MappingProfile>();
    config.AddProfile<GolbonWebRoad.Application.Mapping.MappingProfile>();
}
            );



// 2. جایگزین کردن لاگر پیش‌فرض با Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration) // خواندن تنظیمات از appsettings.json
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());


builder.Services.AddControllersWithViews();

// Payment gateways registration
builder.Services.AddSingleton(provider =>
{
    var cfg = provider.GetRequiredService<IConfiguration>();
    var options = new GolbonWebRoad.Web.Services.Payments.AqayepardakhtOptions
    {
        MerchantId = cfg["Payments:Aqayepardakht:MerchantId"],
        Pin = cfg["Payments:Aqayepardakht:Pin"] ?? "sandbox",
        BaseUrl = cfg["Payments:Aqayepardakht:BaseUrl"] ?? "https://panel.aqayepardakht.ir",
        StartPath = cfg["Payments:Aqayepardakht:StartPath"] ?? "/api/v2/create",
        VerifyPath = cfg["Payments:Aqayepardakht:VerifyPath"] ?? "/api/v2/verify",
        RedirectTemplate = cfg["Payments:Aqayepardakht:RedirectTemplate"] ?? "/startpay/sandbox/{transid}"
    };
    return options;
});
builder.Services.AddHttpClient<GolbonWebRoad.Web.Services.Payments.AqayepardakhtSandboxGateway>()
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls | System.Security.Authentication.SslProtocols.Tls11 | System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13;
        return handler;
    });
builder.Services.AddSingleton<GolbonWebRoad.Web.Services.Payments.IPaymentGateway>(sp =>
    sp.GetRequiredService<GolbonWebRoad.Web.Services.Payments.AqayepardakhtSandboxGateway>());
builder.Services.AddSingleton<GolbonWebRoad.Web.Services.Payments.IPaymentGatewayResolver, GolbonWebRoad.Web.Services.Payments.PaymentGatewayResolver>();

var app = builder.Build();

// 3. اضافه کردن میدل‌ور Serilog برای ثبت اطلاعات درخواست‌های HTTP
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "AdminArea",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await GolbonWebRoad.Infrastructure.DataSeeders.IdentitySeedData.Initialize(userManager, roleManager);

    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occured while sedding the database.");
    }
}

app.Run();



