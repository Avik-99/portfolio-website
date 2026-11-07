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
