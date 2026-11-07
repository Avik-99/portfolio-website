using FluentValidation;

namespace Portfolio.Application.Features.Contact.Commands;

public class SendContactMessageCommandValidator : AbstractValidator<SendContactMessageCommand>
{
    public SendContactMessageCommandValidator()
    {
        RuleFor(c => c.SenderName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(c => c.SenderEmail)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(200);

        RuleFor(c => c.Subject)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(c => c.MessageBody)
            .NotEmpty();
    }
}
