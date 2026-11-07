namespace Portfolio.Application.Common.Options;

public class EmailSettings
{
    public const string SectionName = "EmailSettings";

    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string SenderPassword { get; set; } = string.Empty;
    public string RecipientEmail { get; set; } = string.Empty;
}
