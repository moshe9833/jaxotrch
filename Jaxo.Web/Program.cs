using Jaxo.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<CaseStudyService>();
builder.Services.AddScoped<IEmailService, EmailService>();

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

app.UseStaticFiles();
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
