using System.ComponentModel.DataAnnotations;

namespace Portfolio.Web.ViewModels;

public class ProjectFormViewModel
{
    public Guid Id { get; set; }
    public bool IsEdit => Id != Guid.Empty;

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Category { get; set; } = string.Empty;

    [Display(Name = "Live demo URL")]
    [StringLength(500)]
    public string? LiveDemoUrl { get; set; }

    [Display(Name = "GitHub URL")]
    [StringLength(500)]
    public string? GithubUrl { get; set; }

    [Display(Name = "Image URL")]
    [StringLength(500)]
    public string? ImageUrl { get; set; }

    [Display(Name = "Featured project")]
    public bool IsFeatured { get; set; }

    [Range(0, int.MaxValue)]
    [Display(Name = "Display order")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Technical highlights (one per line)")]
    public string? TechnicalHighlightsText { get; set; }

    [Display(Name = "Tech stack (one per line)")]
    public string? TechStackText { get; set; }

    public static List<string> ParseLines(string? text) =>
        string.IsNullOrWhiteSpace(text)
            ? []
            : text
                .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
}
