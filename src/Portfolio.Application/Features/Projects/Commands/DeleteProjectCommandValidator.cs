using FluentValidation;

namespace Portfolio.Application.Features.Projects.Commands;

public class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    public DeleteProjectCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty();
    }
}
