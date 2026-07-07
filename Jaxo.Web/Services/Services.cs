using System.Text.Json;
using Jaxo.Web.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Jaxo.Web.Services;

public class CaseStudyService
{
    private readonly List<CaseStudy> _studies;

    public CaseStudyService(IWebHostEnvironment env)
    {
        var path = Path.Combine(env.ContentRootPath, "Data", "case-studies.json");
        var json = File.ReadAllText(path);
        _studies = JsonSerializer.Deserialize<List<CaseStudy>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
    }

    public IReadOnlyList<CaseStudy> All => _studies;
    public CaseStudy? BySlug(string slug) =>
        _studies.FirstOrDefault(s => s.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
}

public interface IEmailService
{
    Task<bool> SendContactAsync(ContactForm form);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task<bool> SendContactAsync(ContactForm form)
    {
        var apiKey = _config["SendGrid:ApiKey"];
        var toAddress = _config["SendGrid:ToAddress"] ?? "moshe@jaxotech.com";
        var fromAddress = _config["SendGrid:FromAddress"] ?? "no-reply@jaxotech.com";

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("SendGrid API key not configured; contact form from {Email} logged only. Message: {Message}",
                form.Email, form.Message);
            return true; // dev mode: don't fail the user experience
        }

        var client = new SendGridClient(apiKey);
        var msg = new SendGridMessage
        {
            From = new EmailAddress(fromAddress, "Jaxo website"),
            Subject = $"New project inquiry from {form.Name}",
            PlainTextContent =
                $"Name: {form.Name}\nEmail: {form.Email}\nPhone: {form.Phone}\n\n{form.Message}"
        };
        msg.AddTo(new EmailAddress(toAddress));
        msg.SetReplyTo(new EmailAddress(form.Email, form.Name));

        var response = await client.SendEmailAsync(msg);
        var ok = (int)response.StatusCode < 300;
        if (!ok)
            _logger.LogError("SendGrid send failed with status {Status}", response.StatusCode);
        return ok;
    }
}
