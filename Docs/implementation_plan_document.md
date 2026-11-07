# Implementation Plan Document (IPD)

**Project Name:** Personal Portfolio Web Application  
**Author:** Adib Hasan Avik  
**Version:** 2.0 (Professional Modular Edition)  
**Status:** Approved / Execution Ready  
**Target Environment:** .NET 10 (ASP.NET Core MVC), SQL Server Express (`Avik-99\SQLEXPRESS`), Docker  

---

## 1. Execution Philosophy & Modular Workflow

This document defines an enterprise-grade execution blueprint. Each phase is constructed as a **self-contained execution block**. 

### Execution Command Pattern:
You can issue commands directly to the AI Agent using the standardized syntax:
* `Execute Phase 1: Complete Solution Scaffolding & Domain Setup`
* `Execute Phase 2: Complete Application Layer & CQRS Implementation`
* `Execute Phase 3: Complete Infrastructure, EF Core & Security Implementation`
* `Execute Phase 4: Complete Presentation Layer, UI/UX & Admin Area`
* `Execute Phase 5: Complete Testing, Verification & Dockerization`

---

## 2. Technical Stack & Environment Baseline

| Parameter | Technical Choice / Configuration |
| :--- | :--- |
| **Framework** | .NET 10 (ASP.NET Core MVC) |
| **Architecture** | Clean Architecture (Onion Pattern) with CQRS |
| **Database Instance** | SQL Server Express (`Avik-99\SQLEXPRESS`) |
| **Database Name** | `PortfolioDb` |
| **Connection String** | `Server=Avik-99\SQLEXPRESS;Database=PortfolioDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;` |
| **Global Agent Skills** | `skill-creator`, `frontend-design`, `find-skills` |

---

## 3. Modular Implementation Plan

---

### PHASE 1: Solution Scaffolding & Core Domain Layer
> **Execution Trigger Command:** `Execute Phase 1: Complete Solution Scaffolding & Domain Setup`

#### Objective:
Establish the solution structure with zero-dependency domain primitives, entities, and domain interfaces.

#### Task 1.1: Solution Directory & Project Scaffolding
* Initialize Visual Studio Solution: `PersonalPortfolio.sln`.
* Create `src/Portfolio.Domain` (Class Library, .NET 10).
* Create `src/Portfolio.Application` (Class Library, .NET 10).
* Create `src/Portfolio.Infrastructure` (Class Library, .NET 10).
* Create `src/Portfolio.Web` (ASP.NET Core MVC, .NET 10).
* Create `tests/Portfolio.UnitTests` (NUnit Project, .NET 10).
* Wire project references enforcing Clean Architecture boundaries (Domain has 0 references).

#### Task 1.2: Domain Base Primitives (`Portfolio.Domain/Common/`)
* Implement `BaseEntity<TId>` (abstract with `Id` property).
* Implement `AuditableEntity<TId>` (inherits `BaseEntity`, adds `CreatedAtUtc`, `CreatedBy`, `LastModifiedAtUtc`, `LastModifiedBy`).

#### Task 1.3: Domain Core Entities (`Portfolio.Domain/Entities/`)
* Implement `Project.cs` (Rich domain encapsulation for project highlights, tech stack, demo links, display order).
* Implement `Skill.cs` and `SkillCategory` enum (`BackendAndArchitecture`, `DatabaseAndSecurity`, `Testing`, `DevOpsAndTools`, `Frontend`).
* Implement `ContactMessage.cs` (Sender info, email, message body, IP address, read status).
* Implement `Achievement.cs` (Title, description, date, display order).

#### Task 1.4: Domain Interface Contracts (`Portfolio.Domain/Interfaces/`)
* Implement generic `IRepository<TEntity>` specification contract.
* Implement `IUnitOfWork` interface encapsulating repositories and transaction committing.

#### Verification Criteria for Phase 1:
- Solution builds with 0 errors.
- `Portfolio.Domain` has zero package references or external dependencies.

---

### PHASE 2: Application Layer & CQRS Implementation
> **Execution Trigger Command:** `Execute Phase 2: Complete Application Layer & CQRS Implementation`

#### Objective:
Implement core business logic using MediatR CQRS handlers, DTO definitions, FluentValidation rules, and Mapster mappings.

#### Task 2.1: Package Dependencies Setup
* Add NuGet Packages to `Portfolio.Application`: `MediatR`, `FluentValidation`, `FluentValidation.DependencyInjectionExtensions`, `Mapster`.

#### Task 2.2: Cross-Cutting Behaviors & Mappings (`Portfolio.Application/Common/`)
* Implement `ValidationPipelineBehavior<TRequest, TResponse>` implementing MediatR `IPipelineBehavior`.
* Implement `IEmailService` interface definition.
* Set up Mapster mapping configurations for DTO-Entity bidirectional mapping.

#### Task 2.3: Data Transfer Objects (`Portfolio.Application/DTOs/`)
* Create immutable record DTOs: `ProjectDto`, `SkillDto`, `AchievementDto`, `ContactMessageDto`.

#### Task 2.4: CQRS Feature Handlers (`Portfolio.Application/Features/`)
* **Projects Feature (`Features/Projects`):**
  * Queries: `GetFeaturedProjectsQuery`, `GetAllProjectsQuery`, `GetProjectByIdQuery`.
  * Commands & Validators: `CreateProjectCommand`, `UpdateProjectCommand`, `DeleteProjectCommand`.
* **Skills Feature (`Features/Skills`):**
  * Queries: `GetAllSkillsQuery`.
  * Commands & Validators: `CreateSkillCommand`, `DeleteSkillCommand`.
* **Contact Feature (`Features/Contact`):**
  * Commands & Validators: `SendContactMessageCommand` (triggers `IEmailService`).
  * Queries: `GetAllContactMessagesQuery`.
* **Achievements Feature (`Features/Achievements`):**
  * Queries: `GetAllAchievementsQuery`.
  * Commands: `CreateAchievementCommand`.

#### Verification Criteria for Phase 2:
- Application layer compiles independently.
- MediatR handlers and FluentValidation validators pass mock pipeline verification.

---

### PHASE 3: Infrastructure, EF Core & Security Implementation
> **Execution Trigger Command:** `Execute Phase 3: Complete Infrastructure, EF Core & Security Implementation`

#### Objective:
Implement EF Core data persistence, repository implementations, ASP.NET Core Identity authentication, and initial database seeding.

#### Task 3.1: Package Dependencies & DbContext Setup
* Add NuGet Packages to `Portfolio.Infrastructure`: `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Tools`, `Microsoft.AspNetCore.Identity.EntityFrameworkCore`.
* Create `ApplicationDbContext` inheriting `IdentityDbContext`.
* Implement Fluent API configurations (`ProjectConfiguration`, `SkillConfiguration`, `ContactMessageConfiguration`, `AchievementConfiguration`).
* Configure JSON value conversion for primitive collection properties (`TechnicalHighlights`, `TechStack`).

#### Task 3.2: Repository & Unit of Work Realization
* Implement `Repository<TEntity>` implementing `IRepository<TEntity>`.
* Implement `UnitOfWork` implementing `IUnitOfWork`.

#### Task 3.3: Identity Security & Data Seeder (`Portfolio.Infrastructure/Identity/`)
* Configure ASP.NET Core Identity store.
* Implement `DbInitializer.cs` to apply migrations (`context.Database.MigrateAsync()`) automatically and seed default `Admin` role and initial Admin credentials (`admin@portfolio.local`) on system start.

#### Task 3.4: External Infrastructure Services
* Implement `SmtpEmailService` implementing `IEmailService`.
* Configure Serilog logging sinks (File and Console).

#### Verification Criteria for Phase 3:
- Database migration creates `PortfolioDb` on `Avik-99\SQLEXPRESS`.
- Default Admin user is successfully seeded in the database.

---

### PHASE 4: Presentation Layer, UI/UX & Admin Dashboard
> **Execution Trigger Command:** `Execute Phase 4: Complete Presentation Layer, UI/UX & Admin Area`

#### Objective:
Build a highly responsive ASP.NET Core MVC presentation layer, theme engine, public portfolio, and protected Admin dashboard.

#### Task 4.1: Program Pipeline & Service Lifetime Configuration
* Configure `Program.cs` registering Serilog logging adapter.
* Register Application and Infrastructure services using extension methods (`AddApplicationServices`, `AddInfrastructureServices`).
* Register `ExceptionHandlingMiddleware` for global error handling.

#### Task 4.2: Public Portfolio Portal (`Portfolio.Web/Controllers/ & Views/`)
* Implement `HomeController`:
  * `Index`: Displays Hero Section, About, Categorized Technical Skills, Featured Projects (`MaleFashion`), and Engineering Achievements.
* Implement `ContactController`:
  * `SendMessage` (POST): Accepts AJAX contact requests, validates anti-forgery token (`[ValidateAntiForgeryToken]`), and returns JSON status.

#### Task 4.3: Theme Engine & UI Enhancement
* Integrate Bootstrap 5 / Tailwind CSS.
* Implement Light/Dark mode switcher JavaScript persisting preference to `localStorage`.
* Add resume download link targeting `/assets/resume.pdf`.

#### Task 4.4: Protected Admin Dashboard (`Portfolio.Web/Areas/Admin/`)
* Create `Admin` Area with `AdminController`.
* Apply `[Authorize(Roles = "Admin")]` attribute to secure all routes.
* Views for managing Projects, Skills, and viewing Contact Messages.

#### Verification Criteria for Phase 4:
- Public site renders cleanly on desktop and mobile.
- Dark/Light mode persists across browser refresh.
- Admin dashboard blocks unauthorized access and allows login with seeded credentials.

---

### PHASE 5: Automated Testing, QA & Containerization
> **Execution Trigger Command:** `Execute Phase 5: Complete Testing, Verification & Dockerization`

#### Objective:
Implement complete test coverage and containerize the application for production deployment.

#### Task 5.1: Unit Test Suite (`tests/Portfolio.UnitTests`)
* Add NuGet Packages: `NUnit`, `Moq`, `Shouldly`, `NUnit3TestAdapter`.
* Implement Command Handler Unit Tests (`CreateProjectCommandHandlerTests`, `SendContactMessageCommandHandlerTests`).
* Implement Domain Logic Unit Tests.

#### Task 5.2: Docker Containerization
* Write optimized multi-stage `Dockerfile` targeting .NET 10 runtime.
* Create `docker-compose.yml` orchestrating `Portfolio.Web` container and `SQL Server` database instance.

#### Verification Criteria for Phase 5:
- All unit tests in `Portfolio.UnitTests` pass successfully (`dotnet test`).
- `docker compose up` starts the solution without errors.

---

## 6. Execution Command Summary

| Phase | Target Scope | Command |
| :--- | :--- | :--- |
| **Phase 1** | Solution & Domain Layer | `Execute Phase 1: Complete Solution Scaffolding & Domain Setup` |
| **Phase 2** | CQRS Business Logic | `Execute Phase 2: Complete Application Layer & CQRS Implementation` |
| **Phase 3** | Data & Security | `Execute Phase 3: Complete Infrastructure, EF Core & Security Implementation` |
| **Phase 4** | Web UI & Admin Area | `Execute Phase 4: Complete Presentation Layer, UI/UX & Admin Area` |
| **Phase 5** | QA & Dockerization | `Execute Phase 5: Complete Testing, Verification & Dockerization` |

---
**Document Owner:** Adib Hasan Avik  
**Version:** 2.0 (Professional Modular Edition)  
**Status:** Approved Implementation Plan Baseline