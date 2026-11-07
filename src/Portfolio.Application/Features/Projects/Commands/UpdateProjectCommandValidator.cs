using FluentValidation;

namespace Portfolio.Application.Features.Projects.Commands;

public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty();

        RuleFor(c => c.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(c => c.Description)
            .NotEmpty();

        RuleFor(c => c.Category)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(c => c.LiveDemoUrl)
            .Must(BeValidUrlOrEmpty)
            .WithMessage("Live demo URL must be a valid absolute URL.");

        RuleFor(c => c.GithubUrl)
            .Must(BeValidUrlOrEmpty)
            .WithMessage("GitHub URL must be a valid absolute URL.");

        RuleFor(c => c.ImageUrl)
            .Must(BeValidUrlOrEmpty)
            .WithMessage("Image URL must be a valid absolute URL.");

        RuleFor(c => c.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }

    private static bool BeValidUrlOrEmpty(string? url) =>
        string.IsNullOrWhiteSpace(url) || Uri.TryCreate(url, UriKind.Absolute, out _);
}
