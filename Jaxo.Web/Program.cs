using System.IO.Compression;
using Jaxo.Web.Services;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<CaseStudyService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Compress the render-blocking assets (Bootstrap alone is 233KB raw).
// text/html is deliberately excluded (BREACH) — assets only.
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = new[] { "text/css", "application/javascript", "text/javascript", "image/svg+xml", "application/xml" };
});
builder.Services.Configure<BrotliCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);
builder.Services.Configure<GzipCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// 301 any non-canonical host (e.g. *.azurewebsites.net, www.) to the canonical
// domain so search engines only ever index one copy of the site.
var canonicalHost = app.Configuration["CanonicalHost"];
if (!app.Environment.IsDevelopment() && !string.IsNullOrEmpty(canonicalHost))
{
    app.Use(async (context, next) =>
    {
        var host = context.Request.Host.Host;
        if (!host.Equals(canonicalHost, StringComparison.OrdinalIgnoreCase) &&
            !host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.Redirect(
                $"https://{canonicalHost}{context.Request.PathBase}{context.Request.Path}{context.Request.QueryString}",
                permanent: true);
            return;
        }
        await next();
    });
}

app.UseResponseCompression();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        var path = ctx.Context.Request.Path.Value ?? "";
        // css/js/lib are fingerprinted via asp-append-version; fonts are stable
        ctx.Context.Response.Headers.CacheControl =
            path.StartsWith("/css") || path.StartsWith("/js") || path.StartsWith("/lib") || path.StartsWith("/fonts")
                ? "public,max-age=31536000,immutable"
                : "public,max-age=2592000";
    }
});
app.UseRouting();

app.UseStatusCodePagesWithReExecute("/Home/NotFound");

app.MapControllerRoute(
    name: "sitemap",
    pattern: "sitemap.xml",
    defaults: new { controller = "Home", action = "Sitemap" });

app.MapControllerRoute(
    name: "casestudy",
    pattern: "work/{slug}",
    defaults: new { controller = "Home", action = "CaseStudy" });

app.MapControllerRoute(
    name: "default",
    pattern: "{action=Index}",
    defaults: new { controller = "Home" });

app.Run();
