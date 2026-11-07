using Portfolio.Domain.Common;

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
