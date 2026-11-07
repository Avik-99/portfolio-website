using Mapster;
using MapsterMapper;
using MediatR;
using Moq;
using Portfolio.Application.Common.Mappings;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interfaces;
using Shouldly;

namespace Portfolio.UnitTests.Application.QueryHandlerTests;

[TestFixture]
public class GetAllSkillsQueryHandlerTests
{
    [Test]
    public async Task Handle_ReturnsSkillsOrderedByCategoryThenOrder_WithDisplayName()
    {
        var config = TypeAdapterConfig.GlobalSettings;
        new MappingConfig().Register(config);
        var mapper = new Mapper(config);

        var skills = new List<Skill>
        {
            new("SQL Server", SkillCategory.DatabaseAndSecurity, null, 1),
            new("ASP.NET Core", SkillCategory.BackendAndArchitecture, null, 1),
            new("C#", SkillCategory.BackendAndArchitecture, null, 0),
            new("NUnit", SkillCategory.Testing, null, 0)
        };

        var repository = new Mock<IRepository<Skill>>();
        repository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(skills);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(w => w.Skills).Returns(repository.Object);

        var handler = new Portfolio.Application.Features.Skills.Queries.GetAllSkillsQueryHandler(
            unitOfWork.Object, mapper);

        var result = await handler.Handle(
            new Portfolio.Application.Features.Skills.Queries.GetAllSkillsQuery(),
            CancellationToken.None);

        result.Count.ShouldBe(4);
        result[0].Name.ShouldBe("C#");
        result[0].CategoryName.ShouldBe("Backend & Architecture");
        result[1].Name.ShouldBe("ASP.NET Core");
        result[2].Name.ShouldBe("SQL Server");
        result[2].CategoryName.ShouldBe("Database & Security");
        result[3].Name.ShouldBe("NUnit");
        result[3].CategoryName.ShouldBe("Testing");
    }
}

[TestFixture]
public class GetProjectByIdQueryHandlerTests
{
    [Test]
    public async Task Handle_WhenProjectIsMissing_ReturnsNull()
    {
        var config = TypeAdapterConfig.GlobalSettings;
        new MappingConfig().Register(config);
        var mapper = new Mapper(config);

        var repository = new Mock<IRepository<Project>>();
        repository.Setup(r => r.GetByIdAsync<Guid>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(w => w.Projects).Returns(repository.Object);

        var handler = new Portfolio.Application.Features.Projects.Queries.GetProjectByIdQueryHandler(
            unitOfWork.Object, mapper);

        var result = await handler.Handle(
            new Portfolio.Application.Features.Projects.Queries.GetProjectByIdQuery(Guid.NewGuid()),
            CancellationToken.None);

        result.ShouldBeNull();
    }
}
