# Tech Stack & Engineering Standards Document (Tech.md)

**Project Name:** Personal Portfolio Web Application  
**Author:** Adib Hasan Avik  
**Version:** 1.0  
**Status:** Approved / Architecture Baseline  

---

## 1. Executive Summary

This document defines the technical stack, core libraries, framework components, engineering principles, and coding standards for the Personal Portfolio Web Application. Designed following Clean Architecture principles on **.NET 10**, this document serves as the technical guideline for system implementation, ensuring maintainability, performance, security, and scalability.

---

## 2. Technology Stack Overview

| Category | Technology | Purpose & Usage |
| :--- | :--- | :--- |
| **Framework** | ASP.NET Core MVC (.NET 10) | Web presentation, Controllers, Razor Views |
| **Programming Language** | C# (Latest) | Core application programming |
| **Database Engine** | SQL Server | Persistence storage for dynamic content & Identity |
| **Object-Relational Mapper** | Entity Framework Core (EF Core) | Code-First database migrations & querying |
| **Architecture Pattern** | Clean Architecture / CQRS | Layer separation & decouples read/write workflows |
| **Mediation / Dispatcher** | MediatR | In-process messaging for CQRS Commands & Queries |
| **Data Access Pattern** | Repository & Unit of Work | Centralized data persistence abstraction |
| **Object Mapping** | Mapster | High-performance DTO/Entity/ViewModel mapping |
| **Validation Engine** | FluentValidation | Server-side validation for DTOs and Commands |
| **Structured Logging** | Serilog | Application logging to Console & File sinks |
| **Authentication & Auth** | ASP.NET Core Identity | Claims-Based & Role-Based Admin security |
| **Testing Suite** | NUnit, Moq, Shouldly | Automated unit testing & assertion engine |
| **Containerization** | Docker & Docker Compose | Multi-stage build & local stack orchestration |
| **Frontend Framework** | Bootstrap 5, HTML5, CSS3, JS | Responsive UI & Light/Dark Theme management |

---

## 3. Layered Architecture & Responsibilities

The system is organized into four distinct layers according to Clean Architecture guidelines:

```
                      +----------------------------------+
                      |         Presentation Layer       |
                      |          (Portfolio.Web)         |
                      +----------------------------------+
                                       |
                                       v
                      +----------------------------------+
                      |        Application Layer         |
                      |      (Portfolio.Application)     |
                      +----------------------------------+
                                       |
                                       v
                      +----------------------------------+
                      |          Domain Layer            |
                      |        (Portfolio.Domain)        |
                      +----------------------------------+
                                       ^
                                       |
                      +----------------------------------+
                      |       Infrastructure Layer       |
                      |    (Portfolio.Infrastructure)    |
                      +----------------------------------+
```

### Layer Breakdown:

1. **Portfolio.Domain (Zero Dependencies)**
   - Base entities, value objects, domain logic, and domain exceptions.
   - Domain interface definitions (`IRepository<T>`, `IUnitOfWork`).

2. **Portfolio.Application (Business Logic)**
   - CQRS structure (`Features/Projects`, `Features/Skills`, `Features/Achievements`, `Features/Contact`).
   - MediatR Request handlers, Commands, Queries, and DTOs.
   - FluentValidation behaviors and Mapster mapping configurations.

3. **Portfolio.Infrastructure (Technical Details)**
   - EF Core `ApplicationDbContext` and Fluent API Configurations.
   - Implementation of Repositories and `UnitOfWork`.
   - ASP.NET Core Identity persistence.
   - Email sending implementation (`SmtpEmailService`) and Serilog integration.

4. **Portfolio.Web (Presentation Layer)**
   - ASP.NET Core MVC Controllers (`HomeController`, `AdminController`, `ContactController`).
   - Admin Area (`Areas/Admin`), Razor Views, ViewModels, and Static Assets (`wwwroot`).
   - `Program.cs` service registration and middleware pipeline.

---

## 4. Engineering Standards & Design Principles

* **SOLID Principles:** Strict adherence to Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, and Dependency Inversion.
* **DRY & KISS:** Eliminate redundant logic through generic abstractions, extension methods, and simple solution designs.
* **Composition Over Inheritance:** Prefer interface implementations and dependency injection over heavy inheritance hierarchies.
* **Async/Await Programming:** All I/O-bound operations (Database calls, Email dispatching) must use asynchronous programming model (`async`/`await`).

---

## 5. Security Standards

* **Authentication:** ASP.NET Core Identity with cookie authentication. Public user registration disabled.
* **Authorization:** Role and Claims-based authorization (`[Authorize(Roles = "Admin")]`) for the Admin Dashboard.
* **CSRF Protection:** Anti-Forgery tokens (`[ValidateAntiForgeryToken]`) enforced on all state-changing POST forms.
* **Input Sanitization:** Server-side validation with FluentValidation and safe output encoding via Razor Engine.
* **Credential Safety:** Secrets and connection strings managed through Environment Variables / AppSettings configuration (never hardcoded in source control).

---

## 6. Testing & DevOps Strategy

### Testing
- **NUnit:** Standard test runner.
- **Moq:** Dependency mocking for services and unit testing handlers.
- **Shouldly:** Readable assertion syntax for validation testing.

### DevOps
- **Multi-Stage Dockerfile:** Produces optimized lightweight container images for ASP.NET Core runtime.
- **Docker Compose:** Spawns `Portfolio.Web` and `SQL Server` containers for local production-like environments.

---

**Document Owner:** Adib Hasan Avik  
**Version:** 1.0  
**Status:** Approved Architecture Baseline