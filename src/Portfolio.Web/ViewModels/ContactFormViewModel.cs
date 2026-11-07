using System.ComponentModel.DataAnnotations;

namespace Portfolio.Web.ViewModels;

public class ContactFormViewModel
{
    [Required]
    [StringLength(150)]
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    [Display(Name = "Subject")]
    public string Subject { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Message")]
    public string Message { get; set; } = string.Empty;
}
