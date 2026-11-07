using MediatR;
using Microsoft.Extensions.Options;
using Portfolio.Application.Common.Interfaces;
using Portfolio.Application.Common.Options;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interfaces;

namespace Portfolio.Application.Features.Contact.Commands;

public class SendContactMessageCommandHandler : IRequestHandler<SendContactMessageCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly EmailSettings _emailSettings;

    public SendContactMessageCommandHandler(
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        IOptions<EmailSettings> emailSettings)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _emailSettings = emailSettings.Value;
    }

    public async Task<bool> Handle(SendContactMessageCommand request, CancellationToken cancellationToken)
    {
        var message = new ContactMessage(
            request.SenderName,
            request.SenderEmail,
            request.Subject,
            request.MessageBody,
            request.IpAddress);

        await _unitOfWork.ContactMessages.AddAsync(message, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var subject = $"[Portfolio Contact] {request.Subject}";
        var body =
            $"Name: {request.SenderName}{Environment.NewLine}" +
            $"Email: {request.SenderEmail}{Environment.NewLine}{Environment.NewLine}" +
            request.MessageBody;

        await _emailService.SendEmailAsync(_emailSettings.RecipientEmail, subject, body, cancellationToken);

        return true;
    }
}
