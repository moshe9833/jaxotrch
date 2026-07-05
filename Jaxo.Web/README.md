# Jaxo.Web — jaxotech.com

Brochure site for Jaxo. ASP.NET Core 8 MVC, no database.

## Run locally
    dotnet run
Then open https://localhost:7042

## Configure email (production)
Set the SendGrid key (never commit it):
    dotnet user-secrets init
    dotnet user-secrets set "SendGrid:ApiKey" "SG.xxxxx"
Or in Azure App Service: Configuration > Application settings > SendGrid__ApiKey.
Sender (no-reply@jaxotech.com) must be a verified sender/domain in SendGrid.

## Content
- Case studies: edit Data/case-studies.json (no code changes needed)
- Brand colors: wwwroot/css/site.css (:root variables)
- Logo assets: wwwroot/img (logo.svg, logo-white.svg, ring.svg, favicon.svg)

## Structure
- / (home), /services, /work, /work/{slug}, /about, /contact, /privacy, /terms
- Contact form: honeypot spam trap + server validation + SendGrid
