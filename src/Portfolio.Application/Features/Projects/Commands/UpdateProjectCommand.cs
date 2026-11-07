using MediatR;

namespace Portfolio.Application.Features.Projects.Commands;

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
