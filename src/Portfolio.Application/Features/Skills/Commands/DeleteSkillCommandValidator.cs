using FluentValidation;

namespace Portfolio.Application.Features.Skills.Commands;

public class DeleteSkillCommandValidator : AbstractValidator<DeleteSkillCommand>
{
    public DeleteSkillCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty();
    }
}
