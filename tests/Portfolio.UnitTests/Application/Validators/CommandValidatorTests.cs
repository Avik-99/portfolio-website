using FluentValidation.TestHelper;
using Portfolio.Application.Features.Achievements.Commands;
using Portfolio.Application.Features.Contact.Commands;
using Portfolio.Application.Features.Projects.Commands;
using Shouldly;

namespace Portfolio.UnitTests.Application.Validators;

[TestFixture]
public class CreateProjectCommandValidatorTests
{
    private CreateProjectCommandValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new CreateProjectCommandValidator();

    private static CreateProjectCommand ValidCommand(
        string? liveDemoUrl = null,
        string? githubUrl = null,
        string? imageUrl = null,
        int displayOrder = 0) =>
        new("MaleFashion", "A men's fashion e-commerce app.", "Full-Stack Web Application",
            liveDemoUrl, githubUrl, imageUrl, IsFeatured: true, displayOrder,
            ["Clean Architecture"], ["ASP.NET Core"]);

    [Test]
    public void Validate_ValidCommand_PasssWithoutErrors()
    {
        var result = _validator.TestValidate(ValidCommand(
            liveDemoUrl: "https://example.com/demo",
            githubUrl: "https://github.com/example/repo",
            imageUrl: "https://example.com/cover.png"));

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Validate_EmptyTitle_Fails()
    {
        var result = _validator.TestValidate(ValidCommand() with { Title = "" });

        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(c => c.Title);
    }

    [Test]
    public void Validate_TitleOver200Characters_Fails()
    {
        var result = _validator.TestValidate(ValidCommand() with { Title = new string('x', 201) });

        result.ShouldHaveValidationErrorFor(c => c.Title);
    }

    [Test]
    public void Validate_EmptyDescription_Fails()
    {
        var result = _validator.TestValidate(ValidCommand() with { Description = " " });

        result.ShouldHaveValidationErrorFor(c => c.Description);
    }

    [Test]
    public void Validate_EmptyCategory_Fails()
    {
        var result = _validator.TestValidate(ValidCommand() with { Category = "" });

        result.ShouldHaveValidationErrorFor(c => c.Category);
    }

    [Test]
    public void Validate_MalformedLiveDemoUrl_Fails()
    {
        var result = _validator.TestValidate(ValidCommand(liveDemoUrl: "not a url"));

        result.ShouldHaveValidationErrorFor(c => c.LiveDemoUrl)
            .WithErrorMessage("Live demo URL must be a valid absolute URL.");
    }

    [Test]
    public void Validate_MalformedGithubUrl_Fails()
    {
        var result = _validator.TestValidate(ValidCommand(githubUrl: "not a url"));

        result.ShouldHaveValidationErrorFor(c => c.GithubUrl)
            .WithErrorMessage("GitHub URL must be a valid absolute URL.");
    }

    [Test]
    public void Validate_MalformedImageUrl_Fails()
    {
        var result = _validator.TestValidate(ValidCommand(imageUrl: "www.example.com/no-scheme.png"));

        result.ShouldHaveValidationErrorFor(c => c.ImageUrl);
    }

    [Test]
    public void Validate_NullUrls_AreAllowed()
    {
        var result = _validator.TestValidate(ValidCommand(liveDemoUrl: null, githubUrl: null, imageUrl: null));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_RelativeUrlLikePath_Fails()
    {
        // Uri.TryCreate with UriKind.Absolute rejects bare paths.
        var result = _validator.TestValidate(ValidCommand(liveDemoUrl: "/just/a/path"));

        result.ShouldHaveValidationErrorFor(c => c.LiveDemoUrl);
    }

    [Test]
    public void Validate_NegativeDisplayOrder_Fails()
    {
        var result = _validator.TestValidate(ValidCommand(displayOrder: -1));

        result.ShouldHaveValidationErrorFor(c => c.DisplayOrder);
    }
}

[TestFixture]
public class SendContactMessageCommandValidatorTests
{
    private SendContactMessageCommandValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new SendContactMessageCommandValidator();

    private static SendContactMessageCommand ValidCommand(string email = "jane@example.com", string? ipAddress = null) =>
        new("Jane Doe", email, "Project opportunity", "Hello, are you available?", ipAddress);

    [Test]
    public void Validate_ValidCommand_PasssWithoutErrors()
    {
        var result = _validator.TestValidate(ValidCommand(ipAddress: "10.0.0.1"));

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Validate_EmptySenderName_Fails()
    {
        var result = _validator.TestValidate(ValidCommand() with { SenderName = "" });

        result.ShouldHaveValidationErrorFor(c => c.SenderName);
    }

    [Test]
    public void Validate_SenderNameOver150Characters_Fails()
    {
        var result = _validator.TestValidate(ValidCommand() with { SenderName = new string('n', 151) });

        result.ShouldHaveValidationErrorFor(c => c.SenderName);
    }

    [Test]
    public void Validate_EmptySenderEmail_Fails()
    {
        var result = _validator.TestValidate(ValidCommand(email: ""));

        result.ShouldHaveValidationErrorFor(c => c.SenderEmail);
    }

    [Test]
    public void Validate_InvalidEmailFormat_Fails()
    {
        var result = _validator.TestValidate(ValidCommand(email: "not-an-email"));

        result.ShouldHaveValidationErrorFor(c => c.SenderEmail);
    }

    [Test]
    public void Validate_EmailWithoutAtSign_FailsOnEveryRule()
    {
        var result = _validator.TestValidate(ValidCommand(email: "jane.doe.example.com"));

        result.ShouldHaveValidationErrorFor(c => c.SenderEmail);
    }

    [Test]
    public void Validate_EmptySubject_Fails()
    {
        var result = _validator.TestValidate(ValidCommand() with { Subject = "" });

        result.ShouldHaveValidationErrorFor(c => c.Subject);
    }

    [Test]
    public void Validate_EmptyMessageBody_Fails()
    {
        var result = _validator.TestValidate(ValidCommand() with { MessageBody = "" });

        result.ShouldHaveValidationErrorFor(c => c.MessageBody);
    }

    [Test]
    public void Validate_NullIpAddress_IsAllowed()
    {
        // The web layer passes null when the remote IP cannot be resolved.
        var result = _validator.TestValidate(ValidCommand(ipAddress: null));

        result.IsValid.ShouldBeTrue();
    }
}

[TestFixture]
public class CreateAchievementCommandValidatorTests
{
    private CreateAchievementCommandValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new CreateAchievementCommandValidator();

    private static CreateAchievementCommand ValidCommand(DateTime? date = null, int displayOrder = 0) =>
        new("Docker Milestone", "Containerized the portfolio application.",
            date ?? DateTime.UtcNow.AddDays(-7), displayOrder);

    [Test]
    public void Validate_PastDate_Passes()
    {
        var result = _validator.TestValidate(ValidCommand(DateTime.UtcNow.AddDays(-1)));

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Validate_TodayDate_Passes()
    {
        var result = _validator.TestValidate(ValidCommand(DateTime.UtcNow));

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Validate_FutureDate_Fails()
    {
        var result = _validator.TestValidate(ValidCommand(DateTime.UtcNow.AddDays(2)));

        result.ShouldHaveValidationErrorFor(c => c.AchievementDate)
            .WithErrorMessage("Achievement date must be a valid date in the past.");
    }

    [Test]
    public void Validate_DefaultDate_Fails()
    {
        var result = _validator.TestValidate(ValidCommand() with { AchievementDate = default });

        result.ShouldHaveValidationErrorFor(c => c.AchievementDate);
    }

    [Test]
    public void Validate_EmptyTitle_Fails()
    {
        var result = _validator.TestValidate(ValidCommand() with { Title = "" });

        result.ShouldHaveValidationErrorFor(c => c.Title);
    }

    [Test]
    public void Validate_EmptyDescription_Fails()
    {
        var result = _validator.TestValidate(ValidCommand() with { Description = "" });

        result.ShouldHaveValidationErrorFor(c => c.Description);
    }

    [Test]
    public void Validate_DescriptionOver1000Characters_Fails()
    {
        var result = _validator.TestValidate(ValidCommand() with { Description = new string('d', 1001) });

        result.ShouldHaveValidationErrorFor(c => c.Description);
    }
}
