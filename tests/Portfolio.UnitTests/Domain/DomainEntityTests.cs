using Portfolio.Domain.Entities;
using Shouldly;

namespace Portfolio.UnitTests.Domain;

[TestFixture]
public class ProjectEntityTests
{
    private const string Title = "MaleFashion";
    private const string Description = "A men's fashion e-commerce web application.";
    private const string Category = "Full-Stack E-Commerce Application";
    private const string LiveDemoUrl = "https://example.com/demo";
    private const string GithubUrl = "https://github.com/example";

    [Test]
    public void Constructor_PopulatesIdAndAllProperties()
    {
        var project = new Project(
            Title, Description, Category, LiveDemoUrl, GithubUrl,
            imageUrl: null, isFeatured: true, displayOrder: 1,
            technicalHighlights: ["Clean Architecture"], techStack: ["ASP.NET Core"]);

        project.Id.ShouldNotBe(Guid.Empty);
        project.Title.ShouldBe(Title);
        project.Description.ShouldBe(Description);
        project.Category.ShouldBe(Category);
        project.LiveDemoUrl.ShouldBe(LiveDemoUrl);
        project.GithubUrl.ShouldBe(GithubUrl);
        project.ImageUrl.ShouldBeNull();
        project.IsFeatured.ShouldBeTrue();
        project.DisplayOrder.ShouldBe(1);
        project.TechnicalHighlights.ShouldContain("Clean Architecture");
        project.TechStack.ShouldContain("ASP.NET Core");
    }

    [Test]
    public void Update_ReplacesMutableStateInPlace()
    {
        var project = new Project(Title, Description, Category, null, null, null, false, 0, [], []);

        project.Update("New Title", "New description", "New category", null, null, null, true, 5, ["A"], ["B"]);

        project.Title.ShouldBe("New Title");
        project.IsFeatured.ShouldBeTrue();
        project.DisplayOrder.ShouldBe(5);
        project.TechnicalHighlights.ShouldContain("A");
        project.TechStack.ShouldContain("B");
    }

    [Test]
    public void Update_WithNullCollections_DefaultsToEmptyList()
    {
        var project = new Project(Title, Description, Category, null, null, null, false, 0, [], []);

        project.Update("T", "D", "C", null, null, null, false, 0, null!, null!);

        project.TechnicalHighlights.ShouldNotBeNull();
        project.TechStack.ShouldNotBeNull();
        project.TechnicalHighlights.Count.ShouldBe(0);
        project.TechStack.Count.ShouldBe(0);
    }
}

[TestFixture]
public class SkillEntityTests
{
    [Test]
    public void Constructor_PopulatesIdAndProperties()
    {
        var skill = new Skill("ASP.NET Core", SkillCategory.BackendAndArchitecture, "bi-code-slash", 3);

        skill.Id.ShouldNotBe(Guid.Empty);
        skill.Name.ShouldBe("ASP.NET Core");
        skill.Category.ShouldBe(SkillCategory.BackendAndArchitecture);
        skill.IconClass.ShouldBe("bi-code-slash");
        skill.DisplayOrder.ShouldBe(3);
    }

    [Test]
    public void UpdateDetails_ChangesMutableState()
    {
        var skill = new Skill("Old", SkillCategory.Frontend, null, 0);
        var originalId = skill.Id;

        skill.UpdateDetails("New", SkillCategory.Testing, "bi-check2", 9);

        skill.Id.ShouldBe(originalId);
        skill.Name.ShouldBe("New");
        skill.Category.ShouldBe(SkillCategory.Testing);
        skill.IconClass.ShouldBe("bi-check2");
        skill.DisplayOrder.ShouldBe(9);
    }

    [Test]
    public void SkillCategory_HasExactlyFiveDocumentedValues()
    {
        Enum.GetValues(typeof(SkillCategory)).Length.ShouldBe(5);
    }
}

[TestFixture]
public class ContactMessageEntityTests
{
    [Test]
    public void Constructor_PopulatesMessageAndDefaultsStatus()
    {
        var message = new ContactMessage("Jane Doe", "jane@example.com", "Opportunity", "Hello", "127.0.0.1");

        message.Id.ShouldNotBe(Guid.Empty);
        message.SenderName.ShouldBe("Jane Doe");
        message.SenderEmail.ShouldBe("jane@example.com");
        message.Subject.ShouldBe("Opportunity");
        message.MessageBody.ShouldBe("Hello");
        message.IpAddress.ShouldBe("127.0.0.1");
        message.SubmittedAtUtc.Date.ShouldBe(DateTime.UtcNow.Date);
        message.IsRead.ShouldBeFalse();
    }

    [Test]
    public void MarkAsRead_ElevatesReadStatusOnce()
    {
        var message = new ContactMessage("Jane", "jane@example.com", "S", "B", null);

        message.MarkAsRead();

        message.IsRead.ShouldBeTrue();
    }
}

[TestFixture]
public class AchievementEntityTests
{
    [Test]
    public void Constructor_PopulatesIdAndProperties()
    {
        var date = new DateTime(2025, 6, 15, 0, 0, 0, DateTimeKind.Utc);
        var achievement = new Achievement("Docker", "Containerized the app", date, 2);

        achievement.Id.ShouldNotBe(Guid.Empty);
        achievement.Title.ShouldBe("Docker");
        achievement.Description.ShouldBe("Containerized the app");
        achievement.AchievementDate.ShouldBe(date);
        achievement.DisplayOrder.ShouldBe(2);
    }

    [Test]
    public void Update_ChangesTitleAndOrder()
    {
        var achievement = new Achievement("A", "D", DateTime.UtcNow, 1);
        var originalId = achievement.Id;

        achievement.Update("B", "D2", DateTime.UtcNow, 4);

        achievement.Id.ShouldBe(originalId);
        achievement.Title.ShouldBe("B");
        achievement.DisplayOrder.ShouldBe(4);
    }
}
