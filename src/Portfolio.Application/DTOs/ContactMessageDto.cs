namespace Portfolio.Application.DTOs;

public record ContactMessageDto(
    Guid Id,
    string SenderName,
    string SenderEmail,
    string Subject,
    string MessageBody,
    DateTime SubmittedAtUtc,
    bool IsRead
);
