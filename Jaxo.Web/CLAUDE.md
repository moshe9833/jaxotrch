# CLAUDE.md — Jaxo.Web (jaxotech.com)

## What this is
Brochure/marketing site for Jaxo, the new client-facing brand of MBNC Inc.
ASP.NET Core 8 MVC, no database, single project + sln. Deploys to Azure App Service.

## Brand (do not drift)
- Name: Jaxo (lowercase "jaxo" in the wordmark). Domain: jaxotech.com
- Colors: navy #0B1E3F (anchor), mint #2EE6A8 (accent only — buttons, arcs, the "o"),
  light #F4F6FA. All defined as --jaxo-* CSS variables in wwwroot/css/site.css.
- Logo: "variant B" — the letters "jax" plus a drawn ring "o" (stroke matches font
  weight, mint arc top-right). Inline SVG in _Layout.cshtml; assets in wwwroot/img.
- The ring is the design system: ring bullets (ul.jx-list), progress arcs on the
  Plan/Build/Launch steps, pill buttons. Mint is used sparingly — never as a background.
- Extended ring system (site.css): hero watermark ring (_HeroRing.cshtml partial,
  .jx-hero / .jx-hero-sm), ring glyph before eyebrows (.jx-eyebrow::before), ring "o"
  in display text (.jx-o), card hover corner arc (a.jx-card::before), footer ring
  divider + "Plan. Build. Launch. Full circle." tagline, 404 ring-as-zero, contact
  submit ring spinner, scroll-drawn step arcs (wwwroot/js/site.js + .jx-arc/.jx-check).
- Signature interaction: the hero ring flies up and docks into the logo's "o" as you
  scroll (site.js, scroll-linked so it scrubs both ways; .jx-logo-o spins once on dock;
  disabled for prefers-reduced-motion and below lg).
- Verbal hook: the arc closes when work ships — "full circle" = we launch and stay on.
- Voice: plain language, outcomes not technology, sentence case, no corporate filler.
- Key message (site-wide): AI-assisted development = faster delivery + significantly
  lower cost, always paired with senior-engineer review ("AI writes the first draft,
  engineers own every line"). Ties into bill-by-task: faster tasks = smaller invoices.
  Never invent specific % or $ savings claims — qualitative until Moshe supplies numbers.

## Structure
- One controller (HomeController): Index, Services, Work, CaseStudy(slug), About,
  Contact (GET/POST), ContactThanks, Privacy, Terms, NotFound, Error.
- Routes: /work/{slug} for case studies; everything else is /{action}.
- Case study content: Data/case-studies.json (edit content there, not in views).
- Contact form: server validation + honeypot (Website field) + SendGrid
  (Services/Services.cs). No API key in dev = logs instead of sending.

## Config
- SendGrid key via user-secrets locally, SendGrid__ApiKey app setting in Azure.
- Sender no-reply@jaxotech.com must be verified in SendGrid before go-live.

## Conventions (owner: Moshe, MBNC Inc.)
- Bootstrap 5 + custom CSS on top; no CSS frameworks beyond that, no SPA.
- Keep it a brochure site: no database, no CMS, no blog unless explicitly asked.
- Code-first, concise responses. Bill-by-task mindset: small, reviewable commits.

## Known TODOs
- About page says "20+ years" — confirm real number with Moshe.
- Case studies have qualitative results; replace with real numbers when available.
- Favicon is SVG only; add .ico fallback for old browsers if desired.
- OG/social meta tags not yet added.
