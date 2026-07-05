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
app.UseStaticFiles();
app.UseRouting();

app.UseStatusCodePagesWithReExecute("/Home/NotFound");

app.MapControllerRoute(
    name: "casestudy",
    pattern: "work/{slug}",
    defaults: new { controller = "Home", action = "CaseStudy" });

app.MapControllerRoute(
    name: "default",
    pattern: "{action=Index}",
    defaults: new { controller = "Home" });

app.Run();
