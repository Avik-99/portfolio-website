using Portfolio.Application.DTOs;

namespace Portfolio.Web.ViewModels;

public class HomeViewModel
{
    public IReadOnlyList<ProjectDto> FeaturedProjects { get; set; } = [];
    public Dictionary<string, List<SkillDto>> SkillsCategoryGrouped { get; set; } = new();
    public IReadOnlyList<AchievementDto> Achievements { get; set; } = [];
}
