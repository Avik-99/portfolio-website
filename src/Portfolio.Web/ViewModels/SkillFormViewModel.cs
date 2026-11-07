using System.ComponentModel.DataAnnotations;
using Portfolio.Domain.Entities;

namespace Portfolio.Web.ViewModels;

public class SkillFormViewModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [EnumDataType(typeof(SkillCategory))]
    [Display(Name = "Category")]
    public SkillCategory Category { get; set; }

    [Display(Name = "Icon class (e.g. bi-code-slash)")]
    [StringLength(100)]
    public string? IconClass { get; set; }

    [Range(0, int.MaxValue)]
    [Display(Name = "Display order")]
    public int DisplayOrder { get; set; }
}
