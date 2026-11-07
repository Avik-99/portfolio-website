# AGENTS.md

## Repo state

- **This repo contains only planning documents — no code has been written yet.** `README.md` is one line; everything else lives in `Docs/`.
- When code is added, follow the structure in `Docs/architecture_design_document.md` (solution dir). Do not invent an ad-hoc layout.

## Source of truth in `Docs/`

- `implementation_plan_document.md` — phased execution blueprint (Phases 1–5) with explicit verification criteria per phase. Build in this order; don't skip ahead.
- `tech_stack_engineering_standards.md` — approved tech baseline (version 1.0).
- `personal_Portfolio_prd.md`, `technical_design_document.md` — requirements and feature details.

## Planned stack (from approved docs — follow, don't substitute)

- .NET 10 / ASP.NET Core MVC (not Razor Pages or minimal API), C# latest.
- Clean Architecture, four projects under `src/` plus `tests/Portfolio.UnitTests`:
  `Portfolio.Domain` (zero NuGet packages, zero project refs) → `Portfolio.Application` (reference Domain only) → `Portfolio.Infrastructure` → `Portfolio.Web` (can reference all).
- CQRS via **MediatR**, handlers organized by feature folder (`Features/Projects`, `Features/Skills`, `Features/Achievements`, `Features/Contact`), immutable record DTOs.
- Persistence: EF Core code-first with **Fluent API configurations** in Infrastructure — no data annotations on domain entities. JSON value conversions for primitive collections (`TechnicalHighlights`, `TechStack`).
- Repositories + `IUnitOfWork`; `SaveChangesAsync` happens through UnitOfWork.
- Mapping: **Mapster** (not AutoMapper). Validation: **FluentValidation** wired as a MediatR `ValidationPipelineBehavior`. Logging: **Serilog** (console + file sinks).
- Tests: **NUnit + Moq + Shouldly** (not xUnit; use Shouldly for assertions).
- UI: **Bootstrap 5 only** — confirmed with the owner; the IPD's "Bootstrap 5 / Tailwind CSS" wording is overridden by the approved baseline. Light/dark theme persisted in `localStorage`.

## Confirmed decisions (from owner, not in docs)

- Solution lives at the **repo root**: `PersonalPortfolio.sln`, `src/`, `tests/` beside the existing `Docs/` folder — not in a nested `PersonalPortfolio/` subfolder. Note: `dotnet new sln` on .NET 10 SDK defaults to the new `.slnx` format — use `--format sln` to get the classic `.sln` required by the docs.
- Dev database is the local SQL Server Express instance `Avik-99\SQLEXPRESS` (confirmed installed on this machine); the Docker SQL container is only for containerized runs (Phase 5).
- The seeded admin account (`admin@portfolio.local`) takes its password from configuration (`PORTFOLIO_ADMIN_PASSWORD` env var or user-secrets) — never hardcoded in source.
- `IEmailService` lives in `Portfolio.Application/Common/Interfaces` per the TDD, not in `Domain/Interfaces` as the older architecture-doc file tree suggests.

## Environment specifics

- Target database is a local SQL Server Express named instance: `Avik-99\SQLEXPRESS`, database `PortfolioDb`, using `Trusted_Connection=True` with `TrustServerCertificate=True` (connection string is in the implementation plan, §2).
- `DbInitializer` (Infrastructure/Identity) applies migrations with `Database.MigrateAsync()` and seeds the `Admin` role + `admin@portfolio.local` user at startup — migrations run in code, not just via CLI. The seed password comes from `PORTFOLIO_ADMIN_PASSWORD` (or `IdentitySeed:AdminPassword`); if unset, seeding is skipped with a warning, not an error.
- The Admin account was seeded during Phase 3 verification with a randomly generated local-dev password (delivered in chat at the time; never stored in the repo). Change it via configuration and re-seed, or reset it in the admin UI later.
- EF migrations live in `src/Portfolio.Infrastructure/Persistence/Migrations`; `ApplicationDbContextFactory` (design-time factory) lets `dotnet ef migrations add X --project src/Portfolio.Infrastructure` run without the Web host. The global `dotnet-ef` tool was upgraded to 10.0.14 (was 7.0.12) for this project.
- `Portfolio.Infrastructure` contains a `<FrameworkReference Include="Microsoft.AspNetCore.App" />` — required for `AddIdentity` in a class library; do not remove it.
- Adding packages to a project must run **sequentially** (`;` not parallel calls): concurrent `dotnet add package` on the same `.csproj` can corrupt the file. Old PowerShell 5.1 here does not support `&&`.
- Identity password policy: min 8 chars with digit/upper/lower/non-alphanumeric — seed passwords below that fail with an explicit error.
- Run the site locally with `dotnet run --project src/Portfolio.Web --urls http://localhost:PORT`; on startup it migrates + seeds automatically. Login path `/account/login`, access-denied `/account/denied`, Admin area `admin/*` — the admin login link is in the site navbar.
- Contact form POST `/contact/send` requires the anti-forgery token AND checks a hidden honeypot field `_hp` (silently rejects bots); SMTP unconfigured means the message is persisted but logged-not-sent (no error).
- `wwwroot/assets/resume.pdf` is a generated placeholder; replace with the real resume file.
- Bootstrap 5 is served locally from `wwwroot/lib` (MVC template assets); theme toggle persists `data-bs-theme` choice in localStorage key `portfolio-theme`.
- `tests/Portfolio.UnitTests` references **both** Domain and Application projects (Application needed for handlers; MediatR/FluentValidation/Mapster flow in transitively).
- FluentValidation 12: mock/stand-in validators are easiest as `AbstractValidator<T>` subclasses overriding `ValidateAsync(ValidationContext<T>, ...)`; hand-implementing `IValidator<T>` requires many extra members (Results live in `FluentValidation.Results`). The `TestHelper` validators (`validator.TestValidate(...)`) come from the FluentValidation package itself.
- FluentValidation specifics discovered by tests: `Uri.TryCreate(UriKind.Absolute)` accepts single-slash opaque URIs like `htp:/x` — use clearly non-URL strings in tests; its `EmailAddress` rule is permissive (quoted local parts/space-ish forms may pass); the required `CancellationToken` delegate param on MediatR 12.5's `RequestHandlerDelegate<TResponse>` is optional, so a zero-arg lambda still satisfies it.
- `dotnet test` = 55 tests, all green under Release too (the Docker test stage runs `-c Release --no-build`, so tests must compile warnings-free enough under Release).
- Docker: multi-stage `Dockerfile` + `docker-compose.yml` pairing the web container with a SQL Server container.
- Docker verification facts: the Compose healthcheck MUST use sqlcmd with `-C` (trust self-signed cert) or it fails with an SSL error swallowed by `-o /dev/null`; `MSSQL_SA_PASSWORD` is consumed only at the container's first init (encrypting the data volume) — changing it later requires `docker compose down -v`; avoid `!` in local test passwords (shell/compose escaping hazards + the old PS 5.1 issue); `PORTFOLIO_ADMIN_PASSWORD` must satisfy the Identity policy (digit/upper/lower/non-alphanumeric, min 8) or the web container crash-loops on startup.
- Running the containerized stack locally: set `MSSQL_SA_PASSWORD` and `PORTFOLIO_ADMIN_PASSWORD` host env vars, then `docker compose up -d --build`; the web app comes up at `http://localhost:8080` (SQL's 1433 is bound to `127.0.0.1` only). `docker compose config` validates without the daemon; Docker Desktop's engine must be running for build/up.

## Security rules (hard requirements)

- No public user registration — Identity is admin-only.
- All Admin Area routes protected with `[Authorize(Roles = "Admin")]`.
- `[ValidateAntiForgeryToken]` on every state-changing POST (contact form returns JSON via AJAX).
- Secrets/connection strings via environment variables — never hardcoded.
