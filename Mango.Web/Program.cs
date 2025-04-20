using Mango.Web.Service;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// **Force TLS 1.2 and TLS 1.3**
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

// **Get API URL from Configuration**
SD.CouponAPIBase = builder.Configuration["ServiceUrls:CouponAPI"];
SD.AuthAPIBase = builder.Configuration["ServiceUrls:AuthAPI"];
SD.ProductAPIBase = builder.Configuration["ServiceUrls:ProductAPI"];
if (string.IsNullOrEmpty(SD.AuthAPIBase))
{
    Console.WriteLine("⚠️ Error: CouponAPI Base URL is missing in configuration.");
}
else
{
    Console.WriteLine($"✅ Using Coupon API Base URL: {SD.AuthAPIBase}");
}

// **Create Custom HttpClientHandler with Conditional SSL Bypass**
var httpClientHandler = new HttpClientHandler();
if (builder.Environment.IsDevelopment()) // Only bypass SSL in development mode
{
    Console.WriteLine("⚠️ Warning: SSL certificate validation is disabled in development mode.");
    httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
    {
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            Console.WriteLine($"❌ SSL Error: {sslPolicyErrors}");
            Console.WriteLine($"Issuer: {cert?.Issuer}");
            Console.WriteLine($"Subject: {cert?.Subject}");
            Console.WriteLine($"Valid From: {cert?.GetEffectiveDateString()}");
            Console.WriteLine($"Valid Until: {cert?.GetExpirationDateString()}");
        }
        return true; // Accept all SSL certificates in development
    };
}
else
{
    Console.WriteLine("✅ Running in Production mode: SSL validation is enforced.");
}

// **Register HttpClient with Custom Handler**
builder.Services.AddHttpClient<IAuthService, AuthService>();
builder.Services.AddHttpClient<ICouponService, CouponService>()
    .ConfigurePrimaryHttpMessageHandler(() => httpClientHandler);
builder.Services.AddHttpClient<IProductService, ProductService>()
    .ConfigurePrimaryHttpMessageHandler(() => httpClientHandler);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(10);
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
