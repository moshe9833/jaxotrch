using Jaxo.Web.Models;
using Jaxo.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Jaxo.Web.Controllers;

public class HomeController : Controller
{
    private readonly CaseStudyService _caseStudies;
    private readonly IEmailService _email;
    private readonly IConfiguration _config;

    public HomeController(CaseStudyService caseStudies, IEmailService email, IConfiguration config)
    {
        _caseStudies = caseStudies;
        _email = email;
        _config = config;
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Custom software development, New York";
        ViewData["MetaDescription"] = "Jaxo builds custom software for growing businesses: e-commerce, order management, integrations, and Azure cloud hosting. AI-assisted development means faster delivery at significantly lower cost. Based in Monroe, NY.";
        return View(_caseStudies.All.Take(3).ToList());
    }

    public IActionResult Services()
    {
        ViewData["Title"] = "Custom software development services";
        ViewData["MetaDescription"] = "E-commerce builds, business systems, shipping and payment integrations, and Azure cloud hosting — plain-language services from a New York consultancy that bills by task.";
        return View();
    }

    public IActionResult Work()
    {
        ViewData["Title"] = "Case studies: custom software projects";
        ViewData["MetaDescription"] = "Real custom software projects: in-house order systems, warehouse apps with barcode scanning, e-commerce rescues, and carrier integrations — the problems and results exactly as they happened.";
        return View(_caseStudies.All);
    }

    public IActionResult CaseStudy(string slug)
    {
        var study = _caseStudies.BySlug(slug);
        if (study is null) return NotFound();
        ViewData["Title"] = study.Title;
        ViewData["MetaDescription"] = study.Summary;
        return View(study);
    }

    public IActionResult About()
    {
        ViewData["Title"] = "About Jaxo: a small New York software consultancy";
        ViewData["MetaDescription"] = "Jaxo is a small software consultancy in Monroe, NY. 10+ years building business software, AI-assisted and senior-engineer reviewed. When you call, we answer.";
        return View();
    }

    public IActionResult Sitemap()
    {
        var baseUrl = $"https://{_config["CanonicalHost"] ?? "jaxotech.com"}";
        var paths = new List<string> { "/", "/services", "/work", "/about", "/contact", "/privacy", "/terms" };
        paths.AddRange(_caseStudies.All.Select(c => $"/work/{c.Slug}"));
        var sb = new System.Text.StringBuilder();
        sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.Append("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
        foreach (var p in paths)
            sb.Append($"<url><loc>{baseUrl}{(p == "/" ? "/" : p)}</loc></url>");
        sb.Append("</urlset>");
        return Content(sb.ToString(), "application/xml", System.Text.Encoding.UTF8);
    }

    [HttpGet]
    public IActionResult Contact(bool sent = false)
    {
        ViewData["Title"] = "Get a quote for custom software";
        ViewData["MetaDescription"] = "Tell us what you're trying to build or fix. Written task-by-task estimates, AI-assisted development, and a reply within one business day. Call (845) 662-6703.";
        ViewBag.Sent = sent;
        if (sent) ViewData["Robots"] = "noindex";
        return View(new ContactForm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(ContactForm form)
    {
        ViewData["Title"] = "Get a quote";

        // Honeypot: bots fill hidden fields, humans don't
        if (!string.IsNullOrEmpty(form.Website))
            return RedirectToAction(nameof(Contact), new { sent = true });

        if (!ModelState.IsValid)
        {
            ViewBag.Sent = false;
            return View(form);
        }

        await _email.SendContactAsync(form);
        return RedirectToAction(nameof(Contact), new { sent = true });
    }

    public IActionResult Privacy()
    {
        ViewData["Title"] = "Privacy policy";
        ViewData["MetaDescription"] = "How Jaxo handles your information: what we collect through the contact form, and how to reach us with questions.";
        return View();
    }

    public IActionResult Terms()
    {
        ViewData["Title"] = "Terms of service";
        ViewData["MetaDescription"] = "The terms that apply to using jaxotech.com and working with Jaxo.";
        return View();
    }

    [Route("Home/NotFound")]
    public new IActionResult NotFound()
    {
        Response.StatusCode = 404;
        ViewData["Title"] = "Page not found";
        return View("NotFoundPage");
    }

    public IActionResult Error()
    {
        ViewData["Title"] = "Something went wrong";
        return View();
    }
}
