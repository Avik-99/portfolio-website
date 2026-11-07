using Mapster;
using Portfolio.Application.DTOs;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Common.Mappings;

public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Project, ProjectDto>();

        config.NewConfig<Skill, SkillDto>()
            .Map(d => d.CategoryId, s => (int)s.Category)
            .Map(d => d.CategoryName, s => s.Category.ToString().Replace("And", " & "));

        config.NewConfig<ContactMessage, ContactMessageDto>();

        config.NewConfig<Achievement, AchievementDto>();
    }
}
