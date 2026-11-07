# Technical Design Document (TDD)

**Project Name:** Personal Portfolio Web Application  
**Author:** Adib Hasan Avik  
**Version:** 1.0  
**Status:** Approved / Engineering Baseline  
**Target Framework:** .NET 10 (ASP.NET Core MVC)  

---

## 1. Domain Model & Entity Definitions

The `Portfolio.Domain` project contains zero external dependencies. All entities inherit from abstract base classes to enforce auditability and consistent identity across domain models.

### 1.1 Base Entities & Interfaces

```csharp
namespace Portfolio.Domain.Common;

public abstract class BaseEntity<TId>
{
    public TId Id { get; protected default!; } = default!;
}

public abstract class AuditableEntity<TId> : BaseEntity<TId>
{
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = "System";
    public DateTime? LastModifiedAtUtc { get; set; }
    public string? LastModifiedBy { get; set; }
}
```

### 1.2 Domain Entities & Value Objects

#### 1. `Project` Entity
Represents showcase software projects (e.g., MaleFashion).

```csharp
namespace Portfolio.Domain.Entities;

public class Project : AuditableEntity<Guid>
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public string? LiveDemoUrl { get; private set; }
    public string? GithubUrl { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsFeatured { get; private set; }
    public int DisplayOrder { get; private set; }
    public List<string> TechnicalHighlights { get; private set; } = new();
    public List<string> TechStack { get; private set; } = new();

    private Project() { } // EF Core constructor

    public Project(
        string title, 
        string description, 
        string category, 
        string? liveDemoUrl, 
        string? githubUrl, 
        string? imageUrl, 
        bool isFeatured, 
        int displayOrder, 
        List<string> technicalHighlights, 
        List<string> techStack)
    {
        Id = Guid.NewGuid();
        Update(title, description, category, liveDemoUrl, githubUrl, imageUrl, isFeatured, displayOrder, technicalHighlights, techStack);
    }

    public void Update(
        string title, 
        string description, 
        string category, 
        string? liveDemoUrl, 
        string? githubUrl, 
        string? imageUrl, 
        bool isFeatured, 
        int displayOrder, 
        List<string> technicalHighlights, 
        List<string> techStack)
    {
        Title = title;
        Description = description;
        Category = category;
        LiveDemoUrl = liveDemoUrl;
        GithubUrl = githubUrl;
        ImageUrl = imageUrl;
        IsFeatured = isFeatured;
        DisplayOrder = displayOrder;
        TechnicalHighlights = technicalHighlights ?? new();
        TechStack = techStack ?? new();
    }
}
```

#### 2. `Skill` Entity
Represents technical competencies organized by category.

```csharp
namespace Portfolio.Domain.Entities;

public enum SkillCategory
{
    BackendAndArchitecture = 1,
    DatabaseAndSecurity = 2,
    Testing = 3,
    DevOpsAndTools = 4,
    Frontend = 5
}

public class Skill : AuditableEntity<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public SkillCategory Category { get; private set; }
    public string? IconClass { get; private set; }
    public int DisplayOrder { get; private set; }

    private Skill() { }

    public Skill(string name, SkillCategory category, string? iconClass, int displayOrder)
    {
        Id = Guid.NewGuid();
        Name = name;
        Category = category;
        IconClass = iconClass;
        DisplayOrder = displayOrder;
    }

    public void UpdateDetails(string name, SkillCategory category, string? iconClass, int displayOrder)
    {
        Name = name;
        Category = category;
        IconClass = iconClass;
        DisplayOrder = displayOrder;
    }
}
```

#### 3. `ContactMessage` Entity
Captures messages submitted through the website contact form.

```csharp
namespace Portfolio.Domain.Entities;

public class ContactMessage : BaseEntity<Guid>
{
    public string SenderName { get; private set; } = string.Empty;
    public string SenderEmail { get; private set; } = string.Empty;
    public string Subject { get; private set; } = string.Empty;
    public string MessageBody { get; private set; } = string.Empty;
    public DateTime SubmittedAtUtc { get; private set; } = DateTime.UtcNow;
    public bool IsRead { get; private set; } = false;
    public string? IpAddress { get; private set; }

    private ContactMessage() { }

    public ContactMessage(string senderName, string senderEmail, string subject, string messageBody, string? ipAddress)
    {
        Id = Guid.NewGuid();
        SenderName = senderName;
        SenderEmail = senderEmail;
        Subject = subject;
        MessageBody = messageBody;
        IpAddress = ipAddress;
        SubmittedAtUtc = DateTime.UtcNow;
        IsRead = false;
    }

    public void MarkAsRead() => IsRead = true;
}
```

#### 4. `Achievement` Entity
Represents milestones and technical accomplishments.

```csharp
namespace Portfolio.Domain.Entities;

public class Achievement : AuditableEntity<Guid>
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime AchievementDate { get; private set; }
    public int DisplayOrder { get; private set; }

    private Achievement() { }

    public Achievement(string title, string description, DateTime achievementDate, int displayOrder)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        AchievementDate = achievementDate;
        DisplayOrder = displayOrder;
    }

    public void Update(string title, string description, DateTime achievementDate, int displayOrder)
    {
        Title = title;
        Description = description;
        AchievementDate = achievementDate;
        DisplayOrder = displayOrder;
    }
}
```

---

## 2. Database Schema & Entity Framework Core Design

### 2.1 Schema Mapping Table

| Entity | Table Name | Key Column | Data Types & Constraints | Indexes |
| :--- | :--- | :--- | :--- | :--- |
| `Project` | `Projects` | `Id` (uniqueidentifier) | `Title` NVARCHAR(200) NOT NULL<br>`Category` NVARCHAR(100) NOT NULL<br>`Description` NVARCHAR(MAX) NOT NULL<br>`LiveDemoUrl` NVARCHAR(500) NULL<br>`GithubUrl` NVARCHAR(500) NULL | `IX_Projects_IsFeatured_DisplayOrder`<br>`IX_Projects_Category` |
| `Skill` | `Skills` | `Id` (uniqueidentifier) | `Name` NVARCHAR(100) NOT NULL<br>`Category` INT NOT NULL<br>`IconClass` NVARCHAR(100) NULL | `IX_Skills_Category_DisplayOrder` |
| `ContactMessage` | `ContactMessages` | `Id` (uniqueidentifier) | `SenderName` NVARCHAR(150) NOT NULL<br>`SenderEmail` NVARCHAR(200) NOT NULL<br>`Subject` NVARCHAR(200) NOT NULL<br>`MessageBody` NVARCHAR(MAX) NOT NULL | `IX_ContactMessages_SubmittedAtUtc`<br>`IX_ContactMessages_IsRead` |
| `Achievement` | `Achievements` | `Id` (uniqueidentifier) | `Title` NVARCHAR(200) NOT NULL<br>`Description` NVARCHAR(1000) NOT NULL<br>`AchievementDate` DATETIME2 NOT NULL | `IX_Achievements_DisplayOrder` |

### 2.2 Fluent API Configurations

#### `ProjectConfiguration.cs`
```csharp
namespace Portfolio.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .IsRequired();

        builder.Property(p => p.LiveDemoUrl)
            .HasMaxLength(500);

        builder.Property(p => p.GithubUrl)
            .HasMaxLength(500);

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(500);

        // Storing primitive collections as JSON in SQL Server 2019+
        builder.Property(p => p.TechnicalHighlights)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());

        builder.Property(p => p.TechStack)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());

        builder.HasIndex(p => new { p.IsFeatured, p.DisplayOrder })
            .HasDatabaseName("IX_Projects_IsFeatured_DisplayOrder");
    }
}
```

---

## 3. CQRS Contracts (MediatR Commands, Queries, and DTOs)

### 3.1 DTO Contracts

```csharp
namespace Portfolio.Application.DTOs;

public record ProjectDto(
    Guid Id,
    string Title,
    string Description,
    string Category,
    string? LiveDemoUrl,
    string? GithubUrl,
    string? ImageUrl,
    bool IsFeatured,
    int DisplayOrder,
    List<string> TechnicalHighlights,
    List<string> TechStack
);

public record SkillDto(
    Guid Id,
    string Name,
    string CategoryName,
    int CategoryId,
    string? IconClass,
    int DisplayOrder
);

public record AchievementDto(
    Guid Id,
    string Title,
    string Description,
    DateTime AchievementDate,
    int DisplayOrder
);

public record ContactMessageDto(
    Guid Id,
    string SenderName,
    string SenderEmail,
    string Subject,
    string MessageBody,
    DateTime SubmittedAtUtc,
    bool IsRead
);
```

### 3.2 Commands & Queries Contracts

#### Projects Feature
```csharp
namespace Portfolio.Application.Features.Projects.Queries;

public record GetFeaturedProjectsQuery : IRequest<List<ProjectDto>>;
public record GetAllProjectsQuery : IRequest<List<ProjectDto>>;
public record GetProjectByIdQuery(Guid Id) : IRequest<ProjectDto?>;

namespace Portfolio.Application.Features.Projects.Commands;

public record CreateProjectCommand(
    string Title,
    string Description,
    string Category,
    string? LiveDemoUrl,
    string? GithubUrl,
    string? ImageUrl,
    bool IsFeatured,
    int DisplayOrder,
    List<string> TechnicalHighlights,
    List<string> TechStack
) : IRequest<Guid>;

public record UpdateProjectCommand(
    Guid Id,
    string Title,
    string Description,
    string Category,
    string? LiveDemoUrl,
    string? GithubUrl,
    string? ImageUrl,
    bool IsFeatured,
    int DisplayOrder,
    List<string> TechnicalHighlights,
    List<string> TechStack
) : IRequest<bool>;

public record DeleteProjectCommand(Guid Id) : IRequest<bool>;
```

#### Contact Form Feature
```csharp
namespace Portfolio.Application.Features.Contact.Commands;

public record SendContactMessageCommand(
    string SenderName,
    string SenderEmail,
    string Subject,
    string MessageBody,
    string? IpAddress
) : IRequest<bool>;
```

---

## 4. Service Contracts & Interfaces

### 4.1 `IRepository<T>` Contract
```csharp
namespace Portfolio.Domain.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}
```

### 4.2 `IUnitOfWork` Contract
```csharp
namespace Portfolio.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Project> Projects { get; }
    IRepository<Skill> Skills { get; }
    IRepository<ContactMessage> ContactMessages { get; }
    IRepository<Achievement> Achievements { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

### 4.3 `IEmailService` Contract
```csharp
namespace Portfolio.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default);
}
```

---

## 5. Cross-Cutting Concerns

### 5.1 FluentValidation Behavior Pipeline
```csharp
namespace Portfolio.Application.Common.Behaviors;

public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next();
    }
}
```

### 5.2 Global Exception Handling Middleware
```csharp
namespace Portfolio.Web.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception encountered: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = exception.Message,
            Detailed = context.Response.StatusCode == 500 ? "An unexpected internal server error occurred." : null
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
```

---

## 6. API & Controller Contracts

### 6.1 `HomeController` (Public Portfolio Endpoints)
- **Route:** `/`
- **Security:** Public (`[AllowAnonymous]`)

```csharp
namespace Portfolio.Web.Controllers;

public class HomeController : Controller
{
    private readonly IMediator _mediator;

    public HomeController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var featuredProjects = await _mediator.Send(new GetFeaturedProjectsQuery());
        var skills = await _mediator.Send(new GetAllSkillsQuery());
        var achievements = await _mediator.Send(new GetAllAchievementsQuery());

        var viewModel = new HomeViewModel
        {
            FeaturedProjects = featuredProjects,
            SkillsCategoryGrouped = skills.GroupBy(s => s.CategoryName).ToDictionary(g => g.Key, g => g.ToList()),
            Achievements = achievements
        };

        return View(viewModel);
    }
}
```

### 6.2 `ContactController` (Form Submission)
- **Route:** `/contact`
- **Security:** Anti-Forgery Enforced (`[ValidateAntiForgeryToken]`)

```csharp
namespace Portfolio.Web.Controllers;

public class ContactController : Controller
{
    private readonly IMediator _mediator;

    public ContactController(IMediator mediator) => _mediator = mediator;

    [HttpPost("contact/send")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendMessage(ContactFormViewModel model)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, message = "Validation failed." });

        var command = new SendContactMessageCommand(
            model.Name, 
            model.Email, 
            model.Subject, 
            model.Message, 
            HttpContext.Connection.RemoteIpAddress?.ToString()
        );

        var result = await _mediator.Send(command);

        return Json(new { success = result, message = result ? "Message sent successfully!" : "Failed to send message." });
    }
}
```

### 6.3 `AdminController` (Protected Dashboard)
- **Route:** `/admin/{action}`
- **Security:** Protected by Role Authorization (`[Authorize(Roles = "Admin")]`)

```csharp
namespace Portfolio.Web.Controllers;

[Authorize(Roles = "Admin")]
[Area("Admin")]
[Route("admin")]
public class AdminController : Controller
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator) => _mediator = mediator;

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery());
        var messages = await _mediator.Send(new GetAllContactMessagesQuery());

        return View(new AdminDashboardViewModel { Projects = projects, Messages = messages });
    }

    [HttpPost("projects/create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProject(ProjectCreateViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var command = model.Adapt<CreateProjectCommand>();
        await _mediator.Send(command);

        TempData["SuccessMessage"] = "Project created successfully!";
        return RedirectToAction(nameof(Dashboard));
    }
}
```

---
**Document Owner:** Adib Hasan Avik  
**Version:** 1.0  
**Status:** Approved Technical Design Baseline