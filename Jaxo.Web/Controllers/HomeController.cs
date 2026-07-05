using Jaxo.Web.Models;
using Jaxo.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Jaxo.Web.Controllers;

public class HomeController : Controller
{
    private readonly CaseStudyService _caseStudies;
    private readonly IEmailService _email;

    public HomeController(CaseStudyService caseStudies, IEmailService email)
    {
        _caseStudies = caseStudies;
        _email = email;
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Custom software that runs your business";
        return View(_caseStudies.All.Take(3).ToList());
    }

    public IActionResult Services()
    {
        ViewData["Title"] = "Services";
        return View();
    }

    public IActionResult Work()
    {
        ViewData["Title"] = "Our work";
        return View(_caseStudies.All);
    }

    public IActionResult CaseStudy(string slug)
    {
        var study = _caseStudies.BySlug(slug);
        if (study is null) return NotFound();
        ViewData["Title"] = study.Title;
        return View(study);
    }

    public IActionResult About()
    {
        ViewData["Title"] = "About";
        return View();
    }

    [HttpGet]
    public IActionResult Contact()
    {
        ViewData["Title"] = "Get a quote";
        return View(new ContactForm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(ContactForm form)
    {
        ViewData["Title"] = "Get a quote";

        // Honeypot: bots fill hidden fields, humans don't
        if (!string.IsNullOrEmpty(form.Website))
            return RedirectToAction(nameof(ContactThanks));

        if (!ModelState.IsValid)
            return View(form);

        await _email.SendContactAsync(form);
        return RedirectToAction(nameof(ContactThanks));
    }

    public IActionResult ContactThanks()
    {
        ViewData["Title"] = "Thanks";
        return View();
    }

    public IActionResult Privacy()
    {
        ViewData["Title"] = "Privacy policy";
        return View();
    }

    public IActionResult Terms()
    {
        ViewData["Title"] = "Terms of service";
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
