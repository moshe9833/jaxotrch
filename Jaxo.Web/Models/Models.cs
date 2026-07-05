using System.ComponentModel.DataAnnotations;

namespace Jaxo.Web.Models;

public class ContactForm
{
    [Required(ErrorMessage = "Enter your name")]
    [StringLength(100)]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Enter your email")]
    [EmailAddress(ErrorMessage = "Enter a valid email")]
    [StringLength(200)]
    public string Email { get; set; } = "";

    [StringLength(40)]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Tell us a little about your project")]
    [StringLength(4000)]
    public string Message { get; set; } = "";

    // Honeypot — real users never fill this
    public string? Website { get; set; }
}

public class CaseStudy
{
    public string Slug { get; set; } = "";
    public string Industry { get; set; } = "";
    public string Title { get; set; } = "";
    public string Summary { get; set; } = "";
    public string Problem { get; set; } = "";
    public string Solution { get; set; } = "";
    public string Result { get; set; } = "";
    public List<string> Tags { get; set; } = new();
}
