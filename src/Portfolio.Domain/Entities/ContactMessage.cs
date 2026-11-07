using Portfolio.Domain.Common;

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
