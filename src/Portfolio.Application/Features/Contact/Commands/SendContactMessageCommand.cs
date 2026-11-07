using MediatR;

namespace Portfolio.Application.Features.Contact.Commands;

public record SendContactMessageCommand(
    string SenderName,
    string SenderEmail,
    string Subject,
    string MessageBody,
    string? IpAddress
) : IRequest<bool>;
