using FluentValidation;

namespace Portfolio.Application.Features.Skills.Commands;

public class CreateSkillCommandValidator : AbstractValidator<CreateSkillCommand>
{
    public CreateSkillCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(c => c.Category)
            .IsInEnum();

        RuleFor(c => c.IconClass)
            .MaximumLength(100);

        RuleFor(c => c.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}
