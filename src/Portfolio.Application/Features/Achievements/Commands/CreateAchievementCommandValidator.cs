using FluentValidation;

namespace Portfolio.Application.Features.Achievements.Commands;

public class CreateAchievementCommandValidator : AbstractValidator<CreateAchievementCommand>
{
    public CreateAchievementCommandValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(c => c.Description)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(c => c.AchievementDate)
            .Must(d => d != default && d <= DateTime.UtcNow)
            .WithMessage("Achievement date must be a valid date in the past.");
    }
}
