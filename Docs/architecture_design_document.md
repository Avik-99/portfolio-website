# Architecture Design Document (ADD)

**Project Name:** Personal Portfolio Web Application  
**Author:** Adib Hasan Avik  
**Version:** 1.0  
**Status:** Approved / Architecture Baseline  
**Technology Stack:** ASP.NET Core MVC (.NET 10), EF Core, SQL Server, MediatR, Serilog, Mapster, Docker  

---

## 1. Executive Summary & Architectural Goals

The Personal Portfolio application is built to enterprise-grade software standards. The primary goal of this Architecture Design Document (ADD) is to describe the high-level system design, layer separation, domain boundaries, and data processing workflows.

### Key Architectural Principles:
* **Separation of Concerns (SoC):** Distinct separation between Domain logic, Application workflows, Infrastructure concerns, and UI Presentation.
* **SOLID Principles:** Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, and Dependency Inversion throughout the architecture.
* **DRY (Don't Repeat Yourself):** Reusable components, generic repository patterns, mapping profiles, and centralized error handling.
* **Encapsulation & Domain Integrity:** Protecting internal object states ("encapsulating what changes") and maintaining rich domain boundaries.
* **Composition Over Inheritance:** Favoring interface composition and dependency injection over rigid inheritance hierarchies.
* **OOD Principles:** Strong Object-Oriented Design practices, loose coupling, and high cohesion.

---

## 2. High-Level Architecture Overview (Clean Architecture)

The system enforces a **Clean Architecture** (Onion/Hexagonal Architecture) structure where inner layers have no knowledge of outer layers. Dependencies point inwards.

```
+-----------------------------------------------------------------------+
|                         Presentation Layer                            |
|             (ASP.NET Core MVC, Controllers, Views, UI)                |
+-----------------------------------------------------------------------+
                                   |
                                   v
+-----------------------------------------------------------------------+
|                         Application Layer                             |
|          (CQRS, MediatR Handlers, Commands, Queries, DTOs)            |
+-----------------------------------------------------------------------+
                                   |
                                   v
+-----------------------------------------------------------------------+
|                           Domain Layer                                |
|        (Entities, Domain Exceptions, Interfaces, Value Objects)       |
+-----------------------------------------------------------------------+
                                   ^
                                   |
+-----------------------------------------------------------------------+
|                        Infrastructure Layer                           |
|  (EF Core DbContext, Repositories, Unit of Work, Identity, Serilog)   |
+-----------------------------------------------------------------------+
```

---

## 3. Solution Directory & Layer Structure

```text
PersonalPortfolio/
│
├── src/
│   ├── Portfolio.Domain/                        # Domain Layer (Zero Dependencies)
│   │   ├── Common/                              # Base Entity, Auditable Entities
│   │   ├── Entities/                            # Project, Skill, ContactMessage, Achievement
│   │   └── Interfaces/                          # IRepository<T>, IUnitOfWork, IEmailService
│   │
│   ├── Portfolio.Application/                   # Application Layer (Core Business Logic)
│   │   ├── Features/                            # CQRS Structure by Feature
│   │   │   ├── Projects/                        # Queries & Commands for Projects
│   │   │   └── Contact/                         # CreateContactMessageCommand & Handler
│   │   ├── Common/                              # Interfaces, Mappings (Mapster), Behaviors
│   │   └── DTOs/                                # Request/Response Data Transfer Objects
│   │
│   ├── Portfolio.Infrastructure/                # Infrastructure Layer (Data & External Services)
│   │   ├── Persistence/                         # ApplicationDbContext, EF Configurations
│   │   ├── Repositories/                        # Generic Repository & Specific Repositories
│   │   ├── UnitOfWork/                          # UnitOfWork Implementation
│   │   ├── Identity/                            # ASP.NET Core Identity & Claims Setup
│   │   └── Services/                            # SmtpEmailService, Serilog Logger Adapter
│   │
│   └── Portfolio.Web/                           # Presentation Layer (ASP.NET Core MVC)
│       ├── Controllers/                         # HomeController, AdminController, ContactController
│       ├── ViewModels/                          # UI Binding Models
│       ├── Views/                               # Razor Views & Partial Views
│       ├── wwwroot/                             # Static Files (CSS, JS, Resume PDF)
│       └── Program.cs                           # DI Container, Pipeline & Middleware Configuration
│
├── tests/
│   └── Portfolio.UnitTests/                     # Testing (NUnit, Moq, Shouldly)
│       ├── Application/                         # Command/Query Handler Tests
│       └── Domain/                              # Entity & Logic Validation Tests
│
├── docs/                                        # Project Documentation
│   ├── portfolio_prd.md                         # Product Requirements Document
│   └── portfolio_add.md                         # Architecture Design Document
│
├── Dockerfile                                   # Multi-stage Container Build
├── docker-compose.yml                           # Docker Services Configuration
└── PersonalPortfolio.sln                        # Visual Studio Solution File
```

---

## 4. Key Design Patterns & Technical Implementation

### 4.1 CQRS Pattern with MediatR
* **Command Query Responsibility Segregation (CQRS):** Separates read operations (Queries) from write/mutation operations (Commands).
* **MediatR:** Decouples the MVC Controllers from Application handlers. Controllers send `IRequest<T>` commands/queries to MediatR, which dispatches them to their corresponding `IRequestHandler<TRequest, TResponse>`.

### 4.2 Repository Pattern & Unit of Work
* **Generic Repository (`IRepository<T>`):** Provides standardized CRUD and LINQ specification queries for domain entities.
* **Unit of Work (`IUnitOfWork`):** Manages single database transactions across multiple repository operations ensuring ACID compliance using EF Core `SaveChangesAsync()`.

### 4.3 Data Access & Persistence
* **Entity Framework Core (EF Core):** Uses Code-First migrations with SQL Server.
* **Configurations:** Fluent API mappings implemented in `Infrastructure/Persistence/Configurations` keeping Domain models clean of persistence annotations.

### 4.4 Object Mapping (Mapster)
* **Mapster:** High-performance object-to-object mapping used to map Entities to DTOs and DTOs/Commands to ViewModels without boiler-plate code.

### 4.5 Security & ASP.NET Core Identity
* **Authentication & Authorization:** Built-in ASP.NET Core Identity with Claims-Based Authorization for secure Admin Dashboard management.
* **Data Security:** Anti-Forgery Tokens (`[ValidateAntiForgeryToken]`), client and server-side validation, and input sanitization.

### 4.6 Cross-Cutting Concerns
* **Logging (Serilog):** Centralized structured logging for diagnostics, file logging, and debugging.
* **Dependency Injection (DI):** Service lifetimes (`Transient`, `Scoped`, `Singleton`) registered via extension methods (`ServiceCollectionExtensions`).

---

## 5. Execution & Data Flow Sequence

```text
[User Action] ---> [Razor View / Form]
                         |
                         v
                [MVC Controller]
                         | (Sends Command/Query)
                         v
                    [MediatR]
                         | (Dispatches to Handler)
                         v
           [Command / Query Handler]
                         |
          +--------------+--------------+
          |                             |
          v                             v
  [Mapster Mapping]           [Unit of Work / Repository]
                                        |
                                        v
                               [SQL Server (EF Core)]
```

---

## 6. Testing Strategy

* **Framework:** NUnit.
* **Mocking:** Moq for mocking dependencies (`IUnitOfWork`, `IEmailService`).
* **Assertions:** Shouldly for clear, fluent assertion syntax.
* **Scope:** Unit testing focuses primarily on MediatR Handlers, Domain Validation, and Service layers.

---

## 7. Containerization & Infrastructure

* **Docker Multi-Stage Build:** Optimizes final container size for ASP.NET Core application runtime.
* **Docker Compose:** Orchestrates local environment startup including ASP.NET Core web container and SQL Server database instance.

---
**Document Owner:** Adib Hasan Avik  
**Version:** 1.0  
**Status:** Approved Architecture Baseline
