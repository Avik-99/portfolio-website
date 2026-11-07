using MediatR;
using Moq;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interfaces;
using Shouldly;

namespace Portfolio.UnitTests.Application.CommandHandlerTests;

[TestFixture]
public class CreateSkillCommandHandlerTests
{
    [Test]
    public async Task Handle_AddsSkillAndReturnsId()
    {
        var repository = new Mock<IRepository<Skill>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(w => w.Skills).Returns(repository.Object);
        unitOfWork.Setup(w => w.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new Portfolio.Application.Features.Skills.Commands.CreateSkillCommandHandler(unitOfWork.Object);
        var command = new Portfolio.Application.Features.Skills.Commands.CreateSkillCommand(
            "ASP.NET Core", SkillCategory.BackendAndArchitecture, "bi-code-slash", 3);

        var id = await handler.Handle(command, CancellationToken.None);

        id.ShouldNotBe(Guid.Empty);
        repository.Verify(r => r.AddAsync(
            It.Is<Skill>(s => s.Id == id && s.Name == "ASP.NET Core" && s.Category == SkillCategory.BackendAndArchitecture),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(w => w.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

[TestFixture]
public class DeleteSkillCommandHandlerTests
{
    [Test]
    public async Task Handle_WhenSkillExists_RemovesAndReturnsTrue()
    {
        var skill = new Skill("C#", SkillCategory.BackendAndArchitecture, null, 0);
        var repository = new Mock<IRepository<Skill>>();
        repository.Setup(r => r.GetByIdAsync<Guid>(skill.Id, It.IsAny<CancellationToken>())).ReturnsAsync(skill);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(w => w.Skills).Returns(repository.Object);
        unitOfWork.Setup(w => w.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new Portfolio.Application.Features.Skills.Commands.DeleteSkillCommandHandler(unitOfWork.Object);

        var result = await handler.Handle(
            new Portfolio.Application.Features.Skills.Commands.DeleteSkillCommand(skill.Id),
            CancellationToken.None);

        result.ShouldBeTrue();
        repository.Verify(r => r.Remove(skill), Times.Once);
    }

    [Test]
    public async Task Handle_WhenSkillIsMissing_ThrowsKeyNotFound()
    {
        var repository = new Mock<IRepository<Skill>>();
        repository.Setup(r => r.GetByIdAsync<Guid>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Skill?)null);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(w => w.Skills).Returns(repository.Object);

        var handler = new Portfolio.Application.Features.Skills.Commands.DeleteSkillCommandHandler(unitOfWork.Object);

        await Should.ThrowAsync<KeyNotFoundException>(() => handler.Handle(
            new Portfolio.Application.Features.Skills.Commands.DeleteSkillCommand(Guid.NewGuid()),
            CancellationToken.None));
    }
}

[TestFixture]
public class CreateAchievementCommandHandlerTests
{
    [Test]
    public async Task Handle_AddsAchievementAndReturnsId()
    {
        var repository = new Mock<IRepository<Achievement>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(w => w.Achievements).Returns(repository.Object);
        unitOfWork.Setup(w => w.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new Portfolio.Application.Features.Achievements.Commands.CreateAchievementCommandHandler(unitOfWork.Object);
        var date = DateTime.UtcNow.AddDays(-14);
        var command = new Portfolio.Application.Features.Achievements.Commands.CreateAchievementCommand(
            "Docker", "Containerized the application", date, 2);

        var id = await handler.Handle(command, CancellationToken.None);

        id.ShouldNotBe(Guid.Empty);
        repository.Verify(r => r.AddAsync(
            It.Is<Achievement>(a => a.Id == id && a.AchievementDate == date && a.DisplayOrder == 2),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
