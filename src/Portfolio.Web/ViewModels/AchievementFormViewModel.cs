using System.ComponentModel.DataAnnotations;

namespace Portfolio.Web.ViewModels;

public class AchievementFormViewModel
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Date")]
    public DateTime AchievementDate { get; set; } = DateTime.UtcNow;

    [Range(0, int.MaxValue)]
    [Display(Name = "Display order")]
    public int DisplayOrder { get; set; }
}
